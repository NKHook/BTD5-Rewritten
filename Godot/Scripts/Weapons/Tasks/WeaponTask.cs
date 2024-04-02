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

    /// <summary>
    /// Executes the weapon task
    /// </summary>
    /// <param name="where">The location where the task should be executed</param>
    /// <param name="angle">The angle the task should use, in degrees</param>
    /// <param name="who">What bloon, if any, is relevant to this task</param>
    public abstract void Execute(Vector2 where, float angle, Bloon? who);
    public virtual void Terminate() => QueueFree();
}