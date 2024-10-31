using BloonsTD5Rewritten.Scripts.Bloons;
using BloonsTD5Rewritten.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Weapons.Tasks;

public partial class CollectCollectablesTask : WeaponTask
{
    public float Range;
    public float Speed;
    public float CollectionDelay;
    public bool AnimateOnCollection;
    
    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
    {
        
    }
}