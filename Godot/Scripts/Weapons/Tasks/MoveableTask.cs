using System;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public abstract partial class MoveableTask : WeaponTask
{
    public TaskMovement? Movement;
    public Vector2 Origin;
    public float Angle;

    public Bloon? Target;
    public BaseTower? Sender;
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        Movement?.Move(this, (float)delta);
        switch (Movement?.Type)
        {
            case MovementType.None:
                break;
            case MovementType.Forward:
                if (Origin.DistanceTo(Position) > Movement?.CutOffDistance * 2.5f)
                {
                    Terminate();
                }
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
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override object Clone()
    {
        var clone = base.Clone() as MoveableTask;
        clone!.Movement = Movement?.Clone() as TaskMovement;
        clone.Origin = Origin;
        return clone;
    }
}