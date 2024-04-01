using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public abstract partial class WeaponTask : Node2D, IManagedObject, ICloneable
{
    protected TaskObjectManager? _owner;
    
    public TaskType Type = TaskType.Invalid;
    public WeaponTask[] Tasks = Array.Empty<WeaponTask>();
    
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

    public abstract void Execute(Vector2 where, Vector2 direction, Bloon? who);
    public virtual void Terminate() => QueueFree();
}