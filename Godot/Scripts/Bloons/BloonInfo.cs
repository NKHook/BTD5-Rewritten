using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.Scripts.Weapons;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

public class BloonInfo
{
    public static readonly BloonInfo Invalid = new(BloonType.Invalid);
    
    public BloonType Type { get; set; }
    public BloonDrawLayer DrawLayer { get; set; }
    public long InitialHealth { get; set; }
    public string SpriteFile { get; set; }
    public double BaseSpeed { get; set; }
    public double SpeedMultiplier { get; set; }
    public long Rbe { get; set; }
    public BloonInfo[] ChildBloons { get; set; }
    public StatusFlag StatusImmunity { get; set; }
    public DamageType DamageImmunity { get; set; }
    public bool CanGoUnderground { get; set; }
    public bool RotateToPathDirection { get; set; }
    public double Scale { get; set; }
    public double Radius { get; set; }
    public long HitAddon { get; set; }
    public BloonAbilityFlag BloonAbility { get; set; }

    public BloonInfo()
    {
        Type = BloonType.Invalid;
        DrawLayer = BloonDrawLayer.Invalid;
        SpriteFile = "";
        //SpriteFilesAtDamageLevels = Array.Empty<SpriteFilesAtDamageLevel[]>();
        ChildBloons = Array.Empty<BloonInfo>();
        StatusImmunity = StatusFlag.None;
        DamageImmunity = DamageType.None;
        BloonAbility = BloonAbilityFlag.None;
    }
    public BloonInfo(BloonType bloonType)
    {
        Type = bloonType;
        DrawLayer = BloonDrawLayer.Invalid;
        SpriteFile = "";
        //SpriteFilesAtDamageLevels = Array.Empty<SpriteFilesAtDamageLevel[]>();
        ChildBloons = Array.Empty<BloonInfo>();
        StatusImmunity = StatusFlag.None;
        DamageImmunity = DamageType.None;
        BloonAbility = BloonAbilityFlag.None;
    }
}