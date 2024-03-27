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

    private List<Weapon> _weaponSlots = new();
    private BitArray _activeWeaponSlots = new(64);

    private int _leftUpgrade;
    private int _rightUpgrade;

    private bool _selected;
    private bool _hovered;

    public BaseTower(TowerInfo definition)
    {
        _definition = definition;
        _sprites = definition.GetSprites();
    }

    public override void _Ready()
    {
        var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var mapArea = gameScreen?.GetNode<Area2D>("map_area");
        
        _activeWeaponSlots = new BitArray(_definition.ActiveWeaponSlots);
        _weaponSlots = _definition.GetDefaultWeaponInfo().Select(info => new Weapon(info)).ToList();
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
    }

    private void MapAreaInput(Node viewport, InputEvent @event, long shapeidx)
    {
        var buttonEvent = @event as InputEventMouseButton;
        if ((buttonEvent?.IsPressed() ?? true) && buttonEvent?.ButtonIndex == MouseButton.Left)
        {
            _selected = _hovered;
        }
    }


    private void UpdateSprite()
    {
        var sprite = GetNode<CompoundSprite>("tower_sprite");
        sprite?.Free();

        var newSprite = _sprites.GetSpriteAtUpgrade(_leftUpgrade, _rightUpgrade);
        newSprite.Name = "tower_sprite";
        AddChild(newSprite);
    }
}