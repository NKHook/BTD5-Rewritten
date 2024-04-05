using System;
using System.Linq;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class MultiFireTask : WeaponTask
{
    public float InitialOffset;
    public float AngleIncrement;
    public int FireCount;
    public bool AimAtTarget;
    public Vector2[] Offsets = Array.Empty<Vector2>();
    
    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
    {
        var totalAngle = InitialOffset + AngleIncrement * FireCount;
        var direction = InitialOffset + (AimAtTarget ? angle - totalAngle * 0.5f + 90 : 0.0f);
        for (var i = 0; i < FireCount; i++)
        {
            var offset = (Offsets.Length > i ? Offsets[i] : Vector2.Zero) * 4.0f;
            for (var taskId = 0; taskId < Tasks.Length; taskId++)
            {
                if (DisabledTasks.Contains(taskId)) continue;
                var task = Tasks[taskId];
                
                task.Execute(where + offset, direction - 90, who, user);
            }
            direction += AngleIncrement;
        }
    }
}