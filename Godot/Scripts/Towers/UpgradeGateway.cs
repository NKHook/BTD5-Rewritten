using System.Text.Json.Serialization;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class UpgradeGateway
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Rank")]
    public long? Rank { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("XP")]
    public long? Xp { get; set; }
}