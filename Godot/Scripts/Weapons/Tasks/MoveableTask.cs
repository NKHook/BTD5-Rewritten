using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public abstract partial class MoveableTask : WeaponTask
{
    public TaskMovement? Movement;
    public Vector2 Origin;
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        Movement?.Move(this, (float)delta);
        if (Origin.DistanceTo(Position) > Movement?.CutOffDistance * 2.5f)
        {
            Terminate();
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