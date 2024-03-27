using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Weapons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class BaseTower : Node2D, IManagedObject
{
    private TowerManager? _owner;
    private MapMaskNode? _mapMask;
    private readonly TowerInfo _definition;
    private readonly TowerUpgradeSprites _sprites;

    private List<Weapon?> _weaponSlots = new();
    private BitArray _activeWeaponSlots = new(64);

    private int _leftUpgrade;
    private int _rightUpgrade;

    private bool _selected;
    private bool _hovered;

    public bool Selected => _selected;
    public Area2D? PlacementArea;
    public Shape2D? PlacementShape;

    private static PackedScene? _circle2d;

    public BaseTower(TowerInfo definition)
    {
        _definition = definition;
        _sprites = definition.GetSprites();
    }

    public override void _Ready()
    {
        _circle2d ??= GD.Load<PackedScene>("res://Godot/NewFramework/Circle2D.tscn");

        var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var mapArea = gameScreen?.GetNode<Area2D>("map_area");
        _mapMask = gameScreen?.GetNode<MapMaskNode>("map_mask");

        _activeWeaponSlots = new BitArray(_definition.ActiveWeaponSlots);
        _weaponSlots = _definition.GetDefaultWeaponInfo().Select(info => info == null ? null : new Weapon(info))
            .ToList();
        UpdateSprite();

        PlacementArea = new Area2D();
        var collisionShape = new CollisionShape2D();
        if (_definition.UseRadiusPlacement.GetValueOrDefault(false))
        {
            var radiusShape = new CircleShape2D();
            radiusShape.Radius = _definition.PlacementRadius.GetValueOrDefault(0.0f) * 4.0f;
            collisionShape.Shape = radiusShape;
            PlacementShape = radiusShape;
        }
        else
        {
            var areaShape = new RectangleShape2D();
            areaShape.Size = new Vector2(_definition.PlacementW.GetValueOrDefault(0.0f),
                _definition.PlacementH.GetValueOrDefault(0.0f)) * 4.0f;
            collisionShape.Shape = areaShape;
            PlacementShape = areaShape;
        }

        PlacementArea.AddChild(collisionShape);
        AddChild(PlacementArea);

        mapArea!.InputEvent += MapAreaInput;
        PlacementArea.MouseEntered += () => _hovered = true;
        PlacementArea.MouseExited += () => _hovered = false;

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
        if (_owner!.PlacingTower() && _owner.FloatingTower != this) return;
        
        _selected = true;
        UpdateSelected();
    }

    public void Unselect()
    {
        _selected = false;
        UpdateSelected();
    }

    private void UpdateSelected() => UpdateRadius();

    public Color GetRadiusColor()
    {
        var color = Colors.Black;
        if (AtInvalidPosition())
        {
            color = Colors.Red;
        }
        color.A = 0.25f;
        return color;
    }

    public void UpdateRadius()
    {
        var selectRadiusNode = GetNodeOrNull<Node2D>("select_radius");
        selectRadiusNode?.Free();

        if (!_selected) return;
    
        var range = _definition.UseDefaultRangeCircle.GetValueOrDefault(false) ? 64.0f : GetAttackRange();
        var selectRadius = _circle2d?.Instantiate();
        if (selectRadius == null) return;

        var color = GetRadiusColor();
        selectRadius.Name = "select_radius";
        selectRadius.Set("radius", range * 2.5f);
        selectRadius.Set("color", color);
        AddChild(selectRadius);

        var smallerRadius = _circle2d?.Instantiate();
        if (smallerRadius == null) return;
        smallerRadius.Name = "inner_radius";
        smallerRadius.Set("radius", range * 2.5f - 4.0f);
        smallerRadius.Set("color", color);
        selectRadius.AddChild(smallerRadius);
    }

    public bool AtInvalidPosition()
    {
        var invalid = _owner!.Objects.Any(tower => tower.PlacementArea!.OverlapsArea(PlacementArea));
        if (invalid)
            return invalid;

        if (_mapMask?.MaskData == null)
            return false;

        var maskRelative = _mapMask.MapToMask(Position) / 4;
        if (PlacementShape is CircleShape2D circle)
        {
            var radius = circle.Radius / 4.0f;
            for (var t = 0.0f; t < 2.0f * Math.PI; t += 0.1f)
            {
                var x = Mathf.Sin(t) * radius + maskRelative.X;
                var y = Mathf.Cos(t) * radius + maskRelative.Y;

                if (!_mapMask?.MaskData?.HasPixel((int)x, (int)y) ?? false)
                    continue;
                
                var mask = _mapMask?.MaskData?.GetPixel((int)x, (int)y);
                if ((mask & MapMask.MaskBit.Unplacable) != 0)
                {
                    return true;
                }
            }
        }

        if (PlacementShape is RectangleShape2D rect)
        {
            var size = rect.Size / 4.0f;
            var min = maskRelative - size * 0.5f;
            var max = maskRelative + size * 0.5f;

            for (var x = min.X; x < max.X; x++)
            {
                for (var y = min.Y; y < max.Y; y++)
                {
                    if (!_mapMask?.MaskData?.HasPixel((int)x, (int)y) ?? false)
                        continue;
                    
                    var mask = _mapMask?.MaskData?.GetPixel((int)x, (int)y);
                    if ((mask & MapMask.MaskBit.Unplacable) != 0)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool AtValidPosition() => !AtInvalidPosition();

    private float GetAttackRange()
    {
        if (_weaponSlots.Count == 0)
            return 0.0f;

        var active = GetActiveWeapons().ToList();
        return !active.Any() ? 0.0f : active.Select(weapon => weapon?.Range ?? 0.0f).Max();
    }

    private IEnumerable<Weapon?> GetActiveWeapons()
    {
        return _weaponSlots.Where((weapon, slot) =>
            (_activeWeaponSlots.Count <= slot || _activeWeaponSlots[slot]) && weapon != null);
    }

    private void UpdateSprite()
    {
        var sprite = GetNodeOrNull<CompoundSprite>("tower_sprite");
        sprite?.Free();

        var newSprite = _sprites.GetSpriteAtUpgrade(_leftUpgrade, _rightUpgrade);
        newSprite.Name = "tower_sprite";
        newSprite.Animating = false;
        AddChild(newSprite);
    }

    public void OwnedBy(object? owner)
    {
        _owner = owner as TowerManager;
    }
}