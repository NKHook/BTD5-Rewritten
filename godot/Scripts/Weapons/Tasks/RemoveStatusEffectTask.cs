using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class RemoveStatusEffectTask : WeaponTask
{
    public StatusFlag Status;
    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
    {
        who?.RemoveStatusFlag(Status);
    }
}