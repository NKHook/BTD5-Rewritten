using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public partial class DamageTask : WeaponTask
{
    public DamageType DamageType;
    public int Amount;
    
    public override void Execute(Vector2 where, Vector2 direction, Bloon? who)
    {
        who?.Damage(Amount);
    }
}