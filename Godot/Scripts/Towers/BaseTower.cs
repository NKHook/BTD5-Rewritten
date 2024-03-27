using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Weapons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class BaseTower : Node2D
{
    private readonly TowerInfo _definition;
    private readonly TowerUpgradeSprites _sprites;

    private List<Weapon?> _weaponSlots = new();
    private BitArray _activeWeaponSlots = new(64);

    private int _leftUpgrade;
    private int _rightUpgrade;

    private bool _selected;
    private bool _hovered;

    public bool Selected => _selected;

    private static PackedScene? circle2d;

    public BaseTower(TowerInfo definition)
    {
        _definition = definition;
        _sprites = definition.GetSprites();
    }

    public override void _Ready()
    {
        circle2d ??= GD.Load<PackedScene>("res://Godot/NewFramework/Circle2D.tscn");
        
        var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var mapArea = gameScreen?.GetNode<Area2D>("map_area");
        
        _activeWeaponSlots = new BitArray(_definition.ActiveWeaponSlots);
        _weaponSlots = _definition.GetDefaultWeaponInfo().Select(info => info == null ? null : new Weapon(info)).ToList();
        UpdateSprite();

        var clickTrigger = new Area2D();
        var collisionShape = new CollisionShape2D();
        if (_definition.UseRadiusPlacement.GetValueOrDefault(false))
        {
            var radiusShape = new CircleShape2D();
            radiusShape.Radius = _definition.PlacementRadius.GetValueOrDefault(0.0f) * 4.0f;
            collisionShape.Shape = radiusShape;
        }
        else
        {
            var areaShape = new RectangleShape2D();
            areaShape.Size = new Vector2(_definition.PlacementW.GetValueOrDefault(0.0f),
                _definition.PlacementH.GetValueOrDefault(0.0f)) * 4.0f;
            collisionShape.Shape = areaShape;
        }
        clickTrigger.AddChild(collisionShape);
        AddChild(clickTrigger);
        
        mapArea!.InputEvent += MapAreaInput;
        clickTrigger.MouseEntered += () => _hovered = true;
        clickTrigger.MouseExited += () => _hovered = false;

        ZIndex = 32;
    }

    private void MapAreaInput(Node viewport, InputEvent @event, long shapeidx)
    {
        var buttonEvent = @event as InputEventMouseButton;
        if (!(buttonEvent?.IsPressed() ?? true) || buttonEvent?.ButtonIndex != MouseButton.Left) return;
        
        if (_hovered)
            Select();
        else
            Unselect();
    }

    public void Select()
    {
        _selected = true;
        UpdateSelected();
    }

    public void Unselect()
    {
        _selected = false;
        UpdateSelected();
    }

    private void UpdateSelected()
    {
        var selectRadiusNode = GetNodeOrNull<Node2D>("select_radius");
        switch (_selected)
        {
            case true when selectRadiusNode == null:
            {
                var range = _definition.UseDefaultRangeCircle.GetValueOrDefault(false) ? 64.0f : GetAttackRange();
                var selectRadius = circle2d?.Instantiate();
                if (selectRadius == null) return;

                var color = Colors.Black;
                color.A = 0.25f;
                selectRadius.Name = "select_radius";
                selectRadius.Set("radius", range * 2.5f);
                selectRadius.Set("color", color);
                AddChild(selectRadius);

                var smallerRadius = circle2d?.Instantiate();
                if (smallerRadius == null) return;
                smallerRadius.Name = "inner_radius";
                smallerRadius.Set("radius", (range * 2.5f) - 4.0f);
                smallerRadius.Set("color", color);
                selectRadius.AddChild(smallerRadius);
                
                break;
            }
            case false:
                selectRadiusNode?.QueueFree();
                break;
        }
    }
    
    private float GetAttackRange()
    {
        if (_weaponSlots.Count == 0)
            return 0.0f;
        
        var active = GetActiveWeapons().ToList();
        return !active.Any() ? 0.0f : active.Select(weapon => weapon?.Range ?? 0.0f).Max();
    }

    private IEnumerable<Weapon?> GetActiveWeapons()
    {
        return _weaponSlots.Where((weapon, slot) => (_activeWeaponSlots.Count <= slot || _activeWeaponSlots[slot]) && weapon != null);
    }

    private void UpdateSprite()
    {
        var sprite = GetNode<CompoundSprite>("tower_sprite");
        sprite?.Free();

        var newSprite = _sprites.GetSpriteAtUpgrade(_leftUpgrade, _rightUpgrade);
        newSprite.Name = "tower_sprite";
        newSprite.Animating = false;
        AddChild(newSprite);
    }
}