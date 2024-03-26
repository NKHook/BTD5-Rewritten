using System.Text.Json;
using System.Text.Json.Serialization;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public class WeaponInfo
{
    public static readonly WeaponInfo Invalid = new();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Type")]
    public string Type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TargetRange")]
    public long? TargetRange { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CooldownTime")]
    public long? CooldownTime { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("FireDelayTime")]
    public long? FireDelayTime { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("MaxShots")]
    public long? MaxShots { get; set; }

    public static WeaponInfo FromJson(JsonElement element)
    {
        var weaponInfo = element.Deserialize<WeaponInfo>() ?? Invalid;
        return weaponInfo;
    }
}