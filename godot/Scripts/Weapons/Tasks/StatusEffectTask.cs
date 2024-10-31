using BloonsTD5Rewritten.Scripts.Bloons;
using BloonsTD5Rewritten.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Weapons.Tasks;

public partial class StatusEffectTask : WeaponTask
{
    public StatusFlag Status;
    public float Duration;

    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
    {
        who?.SetStatusFlag(Status);
    }
}