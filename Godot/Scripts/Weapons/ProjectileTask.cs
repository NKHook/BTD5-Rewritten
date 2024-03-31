using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public partial class ProjectileTask : MoveableTask
{
    //Definition info
    public string GraphicName = "";
    public int NumPersists;
    public bool TerminateOnZeroPersists;
    public CollisionType CollisionType;
    public int[] DisabledTasks = Array.Empty<int>();
    public int[] TasksToProcessOnCollision = Array.Empty<int>();
    public int[] TasksToProcessOnTerminate = Array.Empty<int>();

    public override void _Ready()
    {
        base._Ready();

        var sprite = new Sprite();
        sprite.SpriteName = GraphicName;
        sprite.TextureName = "InGame";
        AddChild(sprite);
    }

    public override object Clone()
    {
        var clone = base.Clone() as ProjectileTask;
        clone!.GraphicName = GraphicName;
        clone.NumPersists = NumPersists;
        clone.TerminateOnZeroPersists = TerminateOnZeroPersists;
        clone.CollisionType = CollisionType;
        clone.DisabledTasks = DisabledTasks;
        clone.TasksToProcessOnCollision = TasksToProcessOnCollision;
        clone.TasksToProcessOnTerminate = TasksToProcessOnTerminate;
        return clone;
    }
}