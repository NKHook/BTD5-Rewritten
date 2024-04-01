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
    
    public override void Execute(Vector2 where, Vector2 direction, Bloon? who)
    {
    }
}