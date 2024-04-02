using System;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class ProjectileTask : Tasks.MoveableTask
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
        sprite.RotationDegrees = 90;
        AddChild(sprite);
    }

    public override void Execute(Vector2 where, float angle, Bloon? who)
    {
        var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var taskObjects = gameScreen?.GetNode<TaskObjectManager>("TaskObjects");
        
        if (Clone() is not ProjectileTask clone) return;
        
        clone.Position = where;
        clone.Rotation = Mathf.DegToRad(angle);
        if (clone is MoveableTask { Movement: not null } movable)
        {
            movable.Movement.Direction = Vector2.FromAngle(Mathf.DegToRad(angle));
            movable.Origin = where;
        }
            
        taskObjects?.AddObject(clone);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var bloons = BloonManager.Instance?.Objects;
        if (bloons == null) return;
        
        foreach (var bloon in bloons)
        {
            if (!bloon.Collided(this)) continue;

            foreach (var taskId in TasksToProcessOnCollision)
            {
                if (DisabledTasks.Contains(taskId)) continue;
                    
                var task = Tasks[taskId];
                task.Execute(bloon.Position, 0.0f, bloon);
            }

            switch (CollisionType)
            {
                case CollisionType.Once:
                    Terminate();
                    break;
                case CollisionType.Continual:
                case CollisionType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public override void Terminate()
    {
        base.Terminate();

        foreach (var taskId in TasksToProcessOnTerminate)
        {
            if (DisabledTasks.Contains(taskId)) continue;

            var task = Tasks[taskId];
            task.Execute(Position, RotationDegrees, null);
        }
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