using System;
using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.Scripts.Bloons;
using BloonsTD5Rewritten.Scripts.Bloons.StatusEffects;
using BloonsTD5Rewritten.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Weapons.Tasks;

public partial class StatusEffectTask : WeaponTask
{
    public StatusFlag Status;
    public float Duration;
    public float DamageRate;
    public int NumPersists;
    public float RangeFromTower;
    public bool ShredsMultipleLayers;

    private StatusEffect CreateEffect()
    {
        switch (Status)
        {
            case StatusFlag.DamageOverTime:
            {
                var effect = new DamageOverTimeStatusEffect();
                effect.DamageRate = DamageRate;
                effect.NumPersists = NumPersists;
                effect.RangeFromTower = RangeFromTower;
                return effect;
            }
            case StatusFlag.MultiLayerDamage:
            {
                var effect = new MultiLayerDamageEffect();
                return effect;
            }
            case StatusFlag.ShredBloon:
            {
                var effect = new ShredBloonEffect();
                effect.DamageRate = DamageRate;
                effect.NumPersists = NumPersists;
                effect.RangeFromTower = RangeFromTower;
                effect.ShredsMultipleLayers = ShredsMultipleLayers;
                return effect;
            }
            case StatusFlag.BloonChipperSuck:
            {
                var effect = new BloonChipperSuckEffect();
                effect.NumPersists = NumPersists;
                return effect;
            }
            case StatusFlag.DazeEffect:
            {
                var effect = new DazeEffect();
                effect.NumPersists = NumPersists;
                return effect;
            }
            case StatusFlag.None:
                throw new BTD5WouldCrashException("Attempting to create 'none' status effect.");
            case StatusFlag.Ice:
            case StatusFlag.Glue:
            case StatusFlag.GlueOnTrack:
            case StatusFlag.Napalm:
            case StatusFlag.MoveToPath:
            case StatusFlag.Stun:
            case StatusFlag.CrippleMOAB:
            case StatusFlag.ViralFrost:
            case StatusFlag.IceShards:
            case StatusFlag.Regen:
            case StatusFlag.Camo:
            case StatusFlag.Permafrost:
            case StatusFlag.Slow:
            case StatusFlag.Sabotage:
            case StatusFlag.SignalFlare:
            case StatusFlag.BeeTarget:
            case StatusFlag.BeeSting:
            case StatusFlag.AbsoluteZero:
            case StatusFlag.AbsoluteZeroPermafrost:
            case StatusFlag.Foam:
            case StatusFlag.MoveOnCurve:
            case StatusFlag.VacStatus:
            case StatusFlag.Freeplay:
            case StatusFlag.DamageMultiplier:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
    {
        var effect = CreateEffect();
        effect.Type = Status;
        effect.Duration = Duration;
        effect.Apply(user, who);
    }
}