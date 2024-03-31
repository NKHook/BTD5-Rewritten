using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public partial class MoveableTask : WeaponTask
{
    public TaskMovement? Movement;
    public Vector2 Origin;
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        Movement?.Move(this, (float)delta);
        if (Origin.DistanceTo(Position) > Movement?.CutOffDistance)
        {
            QueueFree();
        }
    }
}