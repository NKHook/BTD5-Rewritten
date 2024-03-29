using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Level;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

public partial class Bloon : Node2D, IManagedObject
{
    private BloonManager? _owner;
    private readonly BloonInfo _definition;

    private PathFollow2D? _pathFollower;
    public int PathOption = 0;

    private float _speed = 1.0f;
    private float _speedMultiplier = 1.0f;

    public Bloon(BloonInfo definition) : base()
    {
        _definition = definition;
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
            AddChild(compound);
        }
        
        //Set up the path follower stuff
        _pathFollower = new PathFollow2D();
        var remote = new RemoteTransform2D();
        remote.RemotePath = GetPath();
        _pathFollower.AddChild(remote);
        _pathFollower.Loop = false;
        _pathFollower.Rotates = _definition.RotateToPathDirection;
        
        bloonPath?.AddChild(_pathFollower);

        _speed = (float)_definition.BaseSpeed!;
        _speedMultiplier = (float)_definition.SpeedMultiplier!;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var progressThisFrame = 3.0f * _speed * _speedMultiplier * (float)delta;
        _pathFollower!.Progress += progressThisFrame;

        if (_pathFollower!.ProgressRatio < 1.0f) return;
        
        _pathFollower.QueueFree();
        QueueFree();
    }

    public void OwnedBy(object? owner)
    {
        _owner = owner as BloonManager;
    }
}