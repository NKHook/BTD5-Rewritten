using System.Text.Json.Serialization;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.GameMode;

public partial class ModeRules
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CanSave")]
    public bool? CanSave { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("PauseBetweenRounds")]
    public bool? PauseBetweenRounds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("BonusTokenRound")]
    public long? BonusTokenRound { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TargetRound")]
    public long? TargetRound { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("DisableHints")]
    public bool? DisableHints { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("DisableEveryplay")]
    public bool? DisableEveryplay { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("BossDamageCash")]
    public long? BossDamageCash { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("AllowContinueOnGameOver")]
    public bool? AllowContinueOnGameOver { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("AllowRetryOnGameOver")]
    public bool? AllowRetryOnGameOver { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("MoneyPerRound")]
    public long? MoneyPerRound { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Reward")]
    public Reward? Reward { get; set; }
}