using System;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons.StatusEffects;

public partial class StatusEffect : Node2D, ICloneable
{
    public StatusFlag Type;
    public int Duration;

    public Bloon? Who;
    public BaseTower? Tower;
    
    public virtual void Apply(BaseTower? tower, Bloon? who)
    {
        Who = who;
        Tower = tower;
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
}