using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using BloonsTD5Rewritten.Godot.Scripts.Level;
using BloonsTD5Rewritten.Godot.Scripts.Weapons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class BaseTower : Node2D, IManagedObject
{
    private TowerManager? _owner;
    private MapMaskNode? _mapMask;
    private readonly TowerInfo _definition;

    private List<Weapon?> _weaponSlots = new();
    private BitArray _activeWeaponSlots = new(64);
    private Node2D? _firePosNode;

    private CompoundSprite? _sprite;

    private int _leftUpgrade;
    private int _rightUpgrade;

    private bool _selected;
    private bool _hovered;
    private bool _animStarted;
    private Vector2? _lastTargetPos;

    public bool Selected => _selected;
    public Area2D? PlacementArea;
    public Shape2D? PlacementShape;

    private static PackedScene? _circle2d;

    public BaseTower(TowerInfo definition)
    {
        _definition = definition;
    }

    public override void _Ready()
    {
        _circle2d ??= GD.Load<PackedScene>("res://NewFramework/Circle2D.tscn");

        var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var mapArea = gameScreen?.GetNode<Area2D>("map_area");
        _mapMask = gameScreen?.GetNode<MapMaskNode>("map_mask");

        _activeWeaponSlots = new BitArray(_definition.ActiveWeaponSlots);
        _weaponSlots = _definition.GetDefaultWeaponInfo().Select(info => info == null ? null : new Weapon(info))
            .ToList();
        UpdateSprite();

        PlacementArea = new Area2D();
        var collisionShape = new CollisionShape2D();
        if (_definition.UseRadiusPlacement)
        {
            var radiusShape = new CircleShape2D();
            radiusShape.Radius = _definition.PlacementRadius * 4.0f;
            collisionShape.Shape = radiusShape;
            PlacementShape = radiusShape;
        }
        else
        {
            var areaShape = new RectangleShape2D();
            areaShape.Size = new Vector2(_definition.PlacementW,
                _definition.PlacementH) * 4.0f;
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

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_sprite is null) return;
        
        foreach (var weaponSlot in _weaponSlots)
        {
            weaponSlot?.UpdateCooldown((float)delta);
        }

        if (!AnyWeaponCooled()) return;
        foreach (var weaponSlot in _weaponSlots)
        {
            weaponSlot?.UpdateFire((float)delta);
        }

        var targets = ValidTargets();
        Bloon? target = null;
        if (targets.Length > 0)
        {
            target = targets.MaxBy(bloon => bloon.Progress);
            _lastTargetPos = target?.GlobalPosition ?? _lastTargetPos;
        }

        if(_definition.RotatesToTarget) RotateToTarget();
        if (_lastTargetPos == null) return;
        
        for (var i = 0; i < _weaponSlots.Count; i++)
        {
            if (_activeWeaponSlots.Length > i && !_activeWeaponSlots[i]) continue;

            var weaponSlot = _weaponSlots[i];
            if (!(weaponSlot?.Cooled ?? false)) continue;
            if (!_animStarted)
            {
                _sprite!.Loop = false;
                _sprite.Animating = true;
                _sprite.Time = 0.0f;
                _animStarted = true;
            }

            if (!weaponSlot.FireReady) continue;

            var firePos = _firePosNode?.GlobalPosition ?? GlobalPosition;
            var offset = _definition.WeaponOffsets.Length > i ? _definition.WeaponOffsets[i] : Vector2.Zero;
            var direction = _definition.RotatesToTarget
                ? firePos.AngleToPoint(_lastTargetPos!.Value)
                : Rotation;
            _animStarted = false;
            weaponSlot.Fire(firePos + offset, Mathf.RadToDeg(direction), target, this);
            _lastTargetPos = null;
        }
    }

    private void RotateToTarget()
    {
        if (_lastTargetPos == null) return;
        LookAt(_lastTargetPos.Value);
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

        var range = GetVisibleRange();
        var selectRadius = _circle2d?.Instantiate();
        if (selectRadius == null) return;

        var color = GetRadiusColor();
        selectRadius.Name = "select_radius";
        selectRadius.Set("radius", range);
        selectRadius.Set("color", color);
        AddChild(selectRadius);

        var smallerRadius = _circle2d?.Instantiate();
        if (smallerRadius == null) return;
        smallerRadius.Name = "inner_radius";
        smallerRadius.Set("radius", range - 4.0f);
        smallerRadius.Set("color", color);
        selectRadius.AddChild(smallerRadius);
    }

    public bool AtInvalidPosition()
    {
        var invalid = _owner!.Objects.Any(tower => tower.PlacementArea!.OverlapsArea(PlacementArea));
        if (invalid)
            return invalid;

        if (_owner.FloatingTower != this)
            return false;

        if (_mapMask?.MaskData == null)
            return false;

        return IsInvalidAt(_mapMask, _definition, Position, _definition.PlacementRadius, _definition.PlacementW, _definition.PlacementH);
    }

    public static bool IsInvalidAt(MapMaskNode mapMask, TowerInfo definition, Vector2 where, float radius = 0.0f, float sizeX = 0.0f, float sizeY = 0.0f)
    {
        var maskRelative = mapMask.MapToMask(where) / 4;
        if (definition.UseRadiusPlacement)
        {
            for (var t = 0.0f; t < 2.0f * Math.PI; t += 0.1f)
            {
                var x = Mathf.Sin(t) * radius + maskRelative.X;
                var y = Mathf.Cos(t) * radius + maskRelative.Y;

                if (!mapMask.MaskData?.HasPixel((int)x, (int)y) ?? false)
                    continue;

                var mask = mapMask.MaskData?.GetPixel((int)x, (int)y);
                if ((mask & MaskBit.BlockTower) != 0)
                {
                    return true;
                }
            }
        }
        else
        {
            var size = new Vector2(sizeX, sizeY);
            var min = maskRelative - size * 0.5f;
            var max = maskRelative + size * 0.5f;

            for (var x = min.X; x < max.X; x++)
            {
                for (var y = min.Y; y < max.Y; y++)
                {
                    if (!mapMask.MaskData?.HasPixel((int)x, (int)y) ?? false)
                        continue;

                    var mask = mapMask.MaskData?.GetPixel((int)x, (int)y);
                    if ((mask & MaskBit.BlockTower) != 0)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool AtValidPosition() => !AtInvalidPosition();

    private float GetVisibleRange()
    {
        return _definition.UseDefaultRangeCircle ? 64.0f * 2.5f : GetAttackRange();
    }
    private float GetAttackRange()
    {
        if (_weaponSlots.Count == 0)
            return 0.0f;

        var active = GetActiveWeapons().ToList();
        return !active.Any()
            ? 0.0f
            : active.Select(weapon => weapon?.Range ?? 0.0f).Max() * 2.5f; //2.5 for whatever reason idek
    }

    private bool AnyWeaponCooled() => GetActiveWeapons().Any(weapon => weapon is { Cooled: true });

    private IEnumerable<Weapon?> GetActiveWeapons()
    {
        return _weaponSlots.Where((weapon, slot) =>
            (_activeWeaponSlots.Count <= slot || _activeWeaponSlots[slot]) && weapon != null);
    }

    private void UpdateSprite()
    {
        if (_sprite == null)
        {
            var sprite = GetNodeOrNull<CompoundSprite>("tower_sprite");
            sprite?.Free();
        }

        _sprite?.Free();

        var newSprite = _definition.GetSprites()?.GetSpriteAtUpgrade(_leftUpgrade, _rightUpgrade) ??
                        throw new BTD5WouldCrashException("Trying to update a sprite that has not been defined");
        newSprite.Name = "tower_sprite";
        newSprite.Animating = false;
        newSprite.Loop = false;
        newSprite.RotationDegrees = 90;
        AddChild(newSprite);
        _sprite = newSprite;
        var firePosArr = newSprite.CustomVariables.ContainsKey("FirePosition")
            ? newSprite.CustomVariables["FirePosition"].Value as float[]
            : Array.Empty<float>();
        if (firePosArr == null || firePosArr.Length == 0) return;

        if (_firePosNode == null)
        {
            _firePosNode = new Node2D();
            AddChild(_firePosNode);
        }
        _firePosNode.Position = new Vector2(-firePosArr[1], firePosArr[0]) * 4.0f;
    }

    private Bloon[] ValidTargets()
    {
        var bloons = BloonManager.Instance?.Objects;
        return bloons?
                   .Where(bloon => bloon.Position.DistanceTo(Position) < GetAttackRange())
                   .ToArray() ??
               Array.Empty<Bloon>();
    }

    public void OwnedBy(object? owner)
    {
        _owner = owner as TowerManager;
    }
}