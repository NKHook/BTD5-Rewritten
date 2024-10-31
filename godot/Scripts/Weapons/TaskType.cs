using System;

namespace BloonsTD5Rewritten.Scripts.Weapons;

[Flags]
public enum TaskType : ulong
{
    Invalid = 0,
    Projectile = 1UL << 0,
    MultiFire = 1UL << 1,
    Damage = 1UL << 2,
    StatusEffect = 1UL << 3,
    RemoveStatusEffect = 1UL << 4,
    AreaOfEffect = 1UL << 5,
    RandomFire = 1UL << 6,
    TimerFire = 1UL << 7,
    Effect = 1UL << 8,
    TextEffect = 1UL << 9,
    BloonSpawnedEvent = 1UL << 10,
    RayIntersect = 1UL << 11,
    ChainTasks = 1UL << 12,
    Collectable = 1UL << 13,
    LaunchAircraft = 1UL << 14,
    TowerModifier = 1UL << 15,
    Harpoon = 1UL << 16,
    Sacrifice = 1UL << 17,
    AddChildSprite = 1UL << 18,
    ChangeTerrain = 1UL << 19,
    CreateTower = 1UL << 20,
    ResetPopCount = 1UL << 21,
    EnableNextWeaponSlot = 1UL << 22,
    EnableWeaponSlot = 1UL << 23,
    ResetCooldown = 1UL << 24,
    RedeployTower = 1UL << 25,
    ChangeSubtaskEnabled = 1UL << 26,
    CollectCollectables = 1UL << 27,
    RestoreAmmo = 1UL << 28,
    FireTaskAtLocation = 1UL << 29,
    OverclockTask = 1UL << 30,
    RBEProjectile = 1UL << 31,
    TrapBloon = 1UL << 32,
    DamageSpread = 1UL << 33,
    TowerBoost = 1UL << 34,
}