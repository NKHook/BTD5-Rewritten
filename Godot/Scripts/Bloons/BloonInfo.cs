using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

public class BloonInfo
{
    public static readonly BloonInfo Invalid = new("INVALID");
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Type")]
    public string Type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("DrawLayer")]
    public string DrawLayer { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("InitialHealth")]
    public long? InitialHealth { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("SpriteFile")]
    public string SpriteFile { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("BaseSpeed")]
    public double? BaseSpeed { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("SpeedMultiplier")]
    public double? SpeedMultiplier { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("RBE")]
    public long? Rbe { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("ChildBloons")]
    public string[] ChildBloons { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("StatusImmunity")]
    public string[] StatusImmunity { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("DamageImmunity")]
    public string[] DamageImmunity { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CanGoUnderground")]
    public bool? CanGoUnderground { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("RotateToPathDirection")]
    public bool? RotateToPathDirection { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Scale")]
    public double? Scale { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Radius")]
    public double? Radius { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("HitAddon")]
    public long? HitAddon { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("BloonAbility")]
    public string[] BloonAbility { get; set; }

    public BloonInfo()
    {
        Type = "INVALID";
        DrawLayer = "";
        SpriteFile = "";
        //SpriteFilesAtDamageLevels = Array.Empty<SpriteFilesAtDamageLevel[]>();
        ChildBloons = Array.Empty<string>();
        StatusImmunity = Array.Empty<string>();
        DamageImmunity = Array.Empty<string>();
        BloonAbility = Array.Empty<string>();
    }
    public BloonInfo(string factoryName)
    {
        Type = factoryName;
        DrawLayer = "";
        SpriteFile = "";
        //SpriteFilesAtDamageLevels = Array.Empty<SpriteFilesAtDamageLevel[]>();
        ChildBloons = Array.Empty<string>();
        StatusImmunity = Array.Empty<string>();
        DamageImmunity = Array.Empty<string>();
        BloonAbility = Array.Empty<string>();
    }
    
    public static BloonInfo FromJson(JsonElement element) => element.Deserialize<BloonInfo>() ?? Invalid;
}