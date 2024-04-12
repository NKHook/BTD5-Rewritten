using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;
using CompoundSprite = BloonsTD5Rewritten.NewFramework.Scripts.Compound.CompoundSprite;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class EffectTask : WeaponTask
{
    public string SpriteFile = "";
    public float EffectScale;
    public WeaponRenderLayer DrawLayer;
    public bool LoopForever;
    public float Duration;

    private CompoundSprite? _sprite;
    private float _time;
    
    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
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
        _sprite.Scale = EffectScale * Vector2.One;
        _sprite.Loop = LoopForever;
        AddChild(_sprite);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _time += (float)delta;
        
        if (_time > (Duration < 0 ? _sprite?.Duration : Duration))
            Terminate();
    }

    public override object Clone()
    {
        var clone = base.Clone() as EffectTask;
        clone!.SpriteFile = SpriteFile;
        clone.EffectScale = EffectScale;
        clone.DrawLayer = DrawLayer;
        clone.LoopForever = LoopForever;
        clone.Duration = Duration;
        return clone;
    }
}