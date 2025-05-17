﻿using System;
using System.Collections.Generic;
using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.Screens;
using BloonsTD5Rewritten.Scripts.Bloons.StatusEffects;
using BloonsTD5Rewritten.Scripts.Level;
using Godot;
using CompoundSprite = BloonsTD5Rewritten.NewFramework.Scripts.Compound.CompoundSprite;

namespace BloonsTD5Rewritten.Scripts.Bloons;

public partial class Bloon : Node2D, IManagedObject
{
    private BloonManager? _owner;
    private readonly BloonInfo _definition;

    public PathFollow2D? PathFollower;
    public int PathOption = 0;
    public float Progress
    {
        get => PathFollower?.ProgressRatio ?? 0.0f;
        private set
        {
            if (PathFollower != null) PathFollower.ProgressRatio = value;
        }
    }

    private int _health = 1;
    private StatusFlag _status = StatusFlag.None;
    private Dictionary<StatusFlag, StatusEffect> _statusEffects = new();

    public EventHandler? BloonReady;
    
    public Bloon(BloonInfo definition)
    {
        _definition = definition;
    }

    private void SetStatusFlag(StatusFlag flag) => _status |= flag;
    private void RemoveStatusFlag(StatusFlag flag) => _status &= ~flag;
    private bool HasStatusFlag(StatusFlag flag) => _status.HasFlag(flag);

    public void AddStatusEffect(StatusEffect effect)
    {
        var effectType = effect.Type;
        if (HasStatusFlag(effectType)) return;
        
        _statusEffects.Add(effectType, effect);
        SetStatusFlag(effectType);
    }

    public void RemoveStatusEffect(StatusFlag effect)
    {
        _statusEffects.Remove(effect);
        RemoveStatusFlag(effect);
    }
    
    private void DisableSpriteProcess(CompoundSprite sprite)
    {
        foreach (var actor in sprite.GetActors())
        {
            if (actor.Node is CompoundSprite compound)
                DisableSpriteProcess(compound);
            
            actor.SetProcess(false);
            actor.Node.SetProcess(false);
        }
    }
    
    public override void _Ready()
    {
        base._Ready();
        
        var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var mapPath = gameScreen?.GetNode<MapPath>("map_path");
        var bloonPath = mapPath?.GetNode<Path2D>("path_" + PathOption);

        //Add the bloon sprite
        var sprite = _definition.SpriteFile;
        if (sprite != string.Empty)
        {
            const string spriteDir = "Assets/JSON/BloonSprites/";
            var spriteFile = spriteDir + sprite + ".json";
            var compound = new CompoundSprite();
            compound.SpriteDefinitionRes = spriteFile;
            compound.Animating = false;
            compound.Scale = Vector2.One * _definition.Scale;
            compound.SetProcess(false);
            AddChild(compound);
            compound.Loaded += (sender, args) => DisableSpriteProcess(compound);
        }

        _health = _definition.InitialHealth;
        
        //Set up the path follower stuff
        PathFollower = new PathFollow2D();
        var remote = new RemoteTransform2D();
        remote.RemotePath = GetPath();
        PathFollower.AddChild(remote);
        PathFollower.Loop = false;
        PathFollower.Rotates = _definition.RotateToPathDirection;
        
        bloonPath?.AddChild(PathFollower);
        
        BloonReady?.Invoke(this, null!);
    }

    public void Damage(int amount)
    {
        _health -= amount;
        if (_health <= 0)
        {
            Pop();
        }
    }

    public void Pop(bool spawnChildren = true)
    {
        Remove();

        if (spawnChildren)
        {
            foreach (var childInfo in _definition.ChildBloons)
            {
                var child = BloonFactory.Instance.Instantiate(childInfo)!;
                child.Progress = Progress;

                if (HasStatusFlag(StatusFlag.MultiLayerDamage) && _health <= 0)
                {
                    child.Damage(_health);
                }
                
                _owner?.AddObject(child);
            }
        }
        
        //TODO: Pop effect
    }

    public void Remove()
    {
        if (_owner != null) _owner.RemoveObject(this);
        else
        {
            PathFollower?.QueueFree();
            QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        if(_health <= 0)
            Pop();
        
        if (HasStatusFlag(StatusFlag.BloonChipperSuck) || HasStatusFlag(StatusFlag.ShredBloon))
            return;
        
        var progressThisFrame = 3.0f * GetBloonSpeed() * (float)delta;
        PathFollower!.Progress += progressThisFrame;

        if (PathFollower!.ProgressRatio < 1.0f) return;
        
        Remove();
    }

    public void OwnedBy(object? owner)
    {
        _owner = owner as BloonManager;
    }

    public bool Collided(Node2D obj) => obj.Position.DistanceTo(Position) < _definition.Radius * 2.5f;
    
    public float GetBloonSpeed() => _definition.BaseSpeed * _definition.SpeedMultiplier;
}