using System;
using System.Linq;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class AreaOfEffectTask : WeaponTask
{
    public float Range;
    public int MaxTargets;
    
    public override void Execute(Vector2 where, Vector2 direction, Bloon? who)
    {
        var bloons = BloonManager.Instance.Objects;
        var targets = bloons.Where(bloon => bloon.Position.DistanceTo(Position) < Range * 2.5f).ToArray();
        for (var i = 0; i < Math.Min(MaxTargets, targets.Length); i++)
        {
            var target = targets[i];

            foreach (var task in Tasks)
            {
                task.Execute(where, direction, target);
            }
        }
    }
}