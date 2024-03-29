using System;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

[Flags]
public enum BloonAbilityFlag
{
    None = 0,
    BloonariusAbility = 1 << 0,
    StunTowersAbility = 1 << 1,
    SlowTowersAbility = 1 << 2,
    ShieldAbility = 1 << 3
}