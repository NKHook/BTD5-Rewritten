using BloonsTD5Rewritten.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Bloons.StatusEffects;

public partial class DamageOverTimeStatusEffect : StatusEffect
{
    public float DamageRate;
    public int NumPersists;
    public int Amount;
    public float RangeFromTower;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Who == null || Tower == null) return;
        
        GD.Print("DOT Process");
    }

    public override object Clone()
    {
        var clone = new DamageOverTimeStatusEffect();
        clone = this;
        return clone;
    }
}