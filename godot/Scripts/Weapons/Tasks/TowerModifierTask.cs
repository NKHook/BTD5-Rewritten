using BloonsTD5Rewritten.Scripts.Bloons;
using BloonsTD5Rewritten.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Weapons.Tasks;

public partial class TowerModifierTask : WeaponTask
{
    public float Range;
    public int PriorityLevel;
    public bool ReplacesPriorityLevel;
    public bool ApplyToUserTower;
    public bool TerminateOnUserUpgrade;
    public float Duration;
    public int NumPersists;
    //public TargetingFilter TargetingFilter
    public TowerModifier? Modifier;
    
    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
    {
        
    }
}