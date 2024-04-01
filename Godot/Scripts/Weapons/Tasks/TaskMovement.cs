using System;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public class TaskMovement : ICloneable
{
    //Definition info
    public MovementType Type;
    public float Speed;
    public int CutOffDistance;

    //Node info
    public Vector2 Direction = Vector2.Zero;
    
    public void Move(Tasks.MoveableTask task, float delta)
    {
        switch (Type)
        {
            case MovementType.None:
                return;
            case MovementType.Forward:
                task.Position += 4.0f * delta * Speed * Direction;
                break;
            case MovementType.RotateAroundTower:
                break;
            case MovementType.GoToTarget:
                break;
            case MovementType.Target:
                break;
            case MovementType.MotionCurve:
                break;
            case MovementType.ReturnToSender:
                break;
            case MovementType.MoveToTouch:
                break;
            case MovementType.BeeMovement:
                break;
            case MovementType.BeeSwarmMovement:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public object Clone()
    {
        var clone = new TaskMovement
        {
            Type = Type,
            Speed = Speed,
            CutOffDistance = CutOffDistance
        };
        return clone;
    }
}