using System;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class MultiFireTask : WeaponTask
{
    public float InitialOffset;
    public float AngleIncrement;
    public int FireCount;
    public bool AimAtTarget;
    public Vector2[] Offsets = Array.Empty<Vector2>();
    
    public override void Execute(Vector2 where, float angle, Bloon? who)
    {
        var totalAngle = InitialOffset + AngleIncrement * FireCount;
        var direction = InitialOffset + (AimAtTarget ? angle - totalAngle * 0.5f : 0.0f);
        for (var i = 0; i < FireCount; i++)
        {
            var offset = (Offsets.Length > i ? Offsets[i] : Vector2.Zero) * 4.0f;
            foreach (var task in Tasks)
            {
                task.Execute(where + offset, direction - 90, who);
            }
            direction += AngleIncrement;
        }
    }
}