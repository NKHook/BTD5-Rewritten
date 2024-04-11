using System;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

[Flags]
public enum DamageType : ulong
{
    None = 0,
    Piercing = 1 << 0,
    Fire = 1 << 1,
    Explosive = 1 << 2,
    Plasma = 1 << 3,
    Juggernaut = 1 << 4,
    MOABMauler = 1 << 5,
    Ice = 1 << 6,
    Foam = 1 << 7
}