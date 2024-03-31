using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public partial class WeaponTask : Node2D, IManagedObject
{
    private TaskObjectManager? _owner;
    
    public TaskType Type = TaskType.Invalid;
    public WeaponTask[] Tasks;
    
    public void OwnedBy(object? owner)
    {
        _owner = owner as TaskObjectManager;
    }
}