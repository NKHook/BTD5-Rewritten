using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

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