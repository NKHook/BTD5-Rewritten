using System;
using BloonsTD5Rewritten.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Bloons.StatusEffects;

public partial class StatusEffect : Node2D, ICloneable
{
    public StatusFlag Type;
    public float Duration;

    public Bloon? Who;
    public BaseTower? Tower;
    
    public virtual void Apply(BaseTower? tower, Bloon? who)
    {
        Who = who;
        Tower = tower;
        who?.AddStatusEffect(this);
    }

    public virtual object Clone()
    {
        var clone = new StatusEffect();
        clone.Type = Type;
        clone.Duration = Duration;
        clone.Who = Who;
        clone.Tower = Tower;
        return clone;
    }

    public virtual float GetBloonSpeed(float speed) => speed;

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        Duration -= (float)delta;
        if (Duration <= 0)
        {
            QueueFree();
        }
    }
}