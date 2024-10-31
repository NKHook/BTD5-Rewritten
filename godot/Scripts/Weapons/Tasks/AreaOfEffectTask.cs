using System;
using System.Linq;
using BloonsTD5Rewritten.Scripts.Bloons;
using BloonsTD5Rewritten.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Weapons.Tasks;

public partial class AreaOfEffectTask : WeaponTask
{
    public float Range;
    public int MaxTargets;
    
    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
    {
        var bloons = BloonManager.Instance!.Objects;
        var targets = bloons.Where(bloon => bloon.Position.DistanceTo(where) < Range * 2.5f).ToArray();
        for (var i = 0; i < Math.Min(MaxTargets, targets.Length); i++)
        {
            var target = targets[i];

            foreach (var task in Tasks)
            {
                task.Execute(where, angle, target, user);
            }
        }
    }
}