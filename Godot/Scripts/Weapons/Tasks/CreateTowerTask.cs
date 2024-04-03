using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class CreateTowerTask : WeaponTask
{
    public TowerType TowerType;
    public Color TowerColor;
    public float TowerLifetime;
    public bool UseParentTowerUpgradeLevel;
    
    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
    {
        
    }
}