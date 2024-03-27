using System.Text.Json;
using System.Text.Json.Serialization;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public class WeaponInfo
{
    public static readonly WeaponInfo Invalid = new();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Type")]
    public string Type = "invalid";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TargetRange")]
    public double? TargetRange { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CooldownTime")]
    public double? CooldownTime { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("FireDelayTime")]
    public double? FireDelayTime { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("MaxShots")]
    public long? MaxShots { get; set; }

    public static WeaponInfo FromJson(JsonElement element)
    {
        var weaponInfo = element.Deserialize<WeaponInfo>() ?? Invalid;
        return weaponInfo;
    }
}