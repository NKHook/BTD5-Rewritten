using System;

namespace BloonsTD5Rewritten.Scripts.Bloons;

[Flags]
public enum StatusFlag : ulong
{
    None = 0,
    Ice = 1 << 0,
    Glue = 1 << 1,
    GlueOnTrack = 1 << 2,
    Napalm = 1 << 3,
    MoveToPath = 1 << 4,
    Stun = 1 << 5,
    CrippleMOAB = 1 << 6,
    ViralFrost = 1 << 7,
    IceShards = 1 << 8,
    Regen = 1 << 9,
    Camo = 1 << 10,
    MultiLayerDamage = 1 << 11,
    Permafrost = 1 << 12,
    Slow = 1 << 13,
    Sabotage = 1 << 14,
    SignalFlare = 1 << 15,
    BeeTarget = 1 << 16,
    BeeSting = 1 << 17,
    AbsoluteZero = 1 << 18,
    AbsoluteZeroPermafrost = 1 << 19,
    Foam = 1 << 20,
    ShredBloon = 1 << 21,
    MoveOnCurve = 1 << 22,
    DazeEffect = 1 << 23,
    VacStatus = 1 << 24,
    Freeplay = 1 << 25,
    BloonChipperSuck = 1 << 26,
    DamageOverTime = 1 << 27,
    DamageMultiplier = 1 << 28
}