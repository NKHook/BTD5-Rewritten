using System;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Weapons.Tasks;

public class TaskMovement : ICloneable
{
    //Definition info
    public MovementType Type;
    public float Speed;
    public float ReturnSpeed;
    public bool TargetShouldFaceWeapon;
    public bool StartOnTarget;
    public int CutOffDistance;
    public Vector2[][] Curves = Array.Empty<Vector2[]>();
    public float AngleOffsets;
    public bool TerminateAtEndOfCurve;
    public bool ScaleCurvesByDirection;

    //Node info
    public Vector2 Direction = Vector2.Zero;
    public float _distance = 0.0f;
    public float _moveFactor = 0.0f;
    private float _t = 0.0f;
    
    private static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        var q0 = p0.Lerp(p1, t);
        var q1 = p1.Lerp(p2, t);
        var q2 = p2.Lerp(p3, t);

        var r0 = q0.Lerp(q1, t);
        var r1 = q1.Lerp(q2, t);

        var s = r0.Lerp(r1, t);
        return s;
    }
    
    public void Move(MoveableTask task, float delta)
    {
        switch (Type)
        {
            case MovementType.None:
                return;
            case MovementType.Forward:
                task.Position += delta * Speed * Direction;
                break;
            case MovementType.RotateAroundTower:
                break;
            case MovementType.GoToTarget:
                break;
            case MovementType.Target:
                break;
            case MovementType.MotionCurve:
                _moveFactor = 0.01f;
                if (Mathf.FloorToInt(_t) >= Curves.Length)
                {
                    if(TerminateAtEndOfCurve)
                        task.Terminate();
                    _t = 0.0f;
                }
                var curveId = Mathf.FloorToInt(_t);
                var curve = Curves[curveId];
                var pos =
                    CubicBezier(
                        curve[0] * 4.0f,
                        curve[1] * 4.0f,
                        curve[2] * 4.0f,
                        curve[3] * 4.0f,
                        _t - curveId);
                task.Position = pos.Rotated(Mathf.DegToRad(task.Angle)) + task.Origin;
                _t += Speed * _moveFactor * delta;
                break;
            case MovementType.ReturnToSender:
                if (StartOnTarget && _t <= 0.0f)
                {
                    task.Position = task.Target?.Position ?? Vector2.Zero;
                    _distance = task.Position.DistanceTo(task.Sender?.Position ?? Vector2.Zero);
                    _moveFactor = 1.0f / _distance;
                }
                if (task.Target != null)
                {
                    task.Target.Position = task.Position.Lerp(task.Sender?.Position ?? Vector2.Zero, _t);
                }
                _t += ReturnSpeed * _moveFactor * delta;
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
            ReturnSpeed = ReturnSpeed,
            TargetShouldFaceWeapon = TargetShouldFaceWeapon,
            StartOnTarget = StartOnTarget,
            CutOffDistance = CutOffDistance,
            Curves = Curves,
            AngleOffsets = AngleOffsets,
            TerminateAtEndOfCurve = TerminateAtEndOfCurve,
            ScaleCurvesByDirection = ScaleCurvesByDirection
        };
        return clone;
    }
}