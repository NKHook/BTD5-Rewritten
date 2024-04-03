using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class EffectTask : WeaponTask
{
    public string SpriteFile;
    public float Scale;
    public WeaponRenderLayer DrawLayer;
    public bool LoopForever;
    public float Duration;

    private CompoundSprite? _sprite;
    
    public override void Execute(Vector2 where, float angle, Bloon? who)
    {
        var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var taskObjects = gameScreen?.GetNode<TaskObjectManager>("TaskObjects");
        
        if (Clone() is not EffectTask clone) return;
        
        clone.Position = where;
            
        taskObjects?.AddObject(clone);;
    }

    public override void _Ready()
    {
        base._Ready();

        const string weaponSpritesDir = "Assets/JSON/WeaponSprites/";
        _sprite = new CompoundSprite();
        _sprite.SpriteDefinitionRes = weaponSpritesDir + SpriteFile;
        _sprite.Scale = Scale * Vector2.One;
        _sprite.Loop = LoopForever;
        AddChild(_sprite);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        if (_sprite?.Time > (Duration < 0 ? _sprite?.Duration : Duration))
            Terminate();
    }
}