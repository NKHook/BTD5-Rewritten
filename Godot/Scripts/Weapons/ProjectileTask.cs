using System;
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
}