using System;
using BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public class WeaponInfo
{
    public static readonly WeaponInfo Invalid = new();

    public WeaponType Type = WeaponType.Invalid;
    public float TargetRange { get; set; }
    public float CooldownTime { get; set; }
    public float FireDelayTime { get; set; }
    public int MaxShots { get; set; }

    public WeaponTask[] Tasks = Array.Empty<WeaponTask>();
}