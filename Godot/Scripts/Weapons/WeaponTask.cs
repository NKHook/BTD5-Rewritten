using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public partial class WeaponTask : Node2D, IManagedObject, ICloneable
{
    protected TaskObjectManager? _owner;
    
    public TaskType Type = TaskType.Invalid;
    public WeaponTask[] Tasks;
    
    public void OwnedBy(object? owner)
    {
        _owner = owner as TaskObjectManager;
    }

    public virtual object Clone()
    {
        var clone = Duplicate() as WeaponTask;
        clone!.Type = Type;
        clone.Tasks = Tasks;
        return clone;
    }
}