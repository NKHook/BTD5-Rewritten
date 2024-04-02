using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class DamageTask : WeaponTask
{
    public DamageType DamageType;
    public int Amount;
    
    public override void Execute(Vector2 where, float angle, Bloon? who)
    {
        who?.Damage(Amount);
    }
}