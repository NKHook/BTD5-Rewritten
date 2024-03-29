using System.Text.Json.Serialization;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.GameMode;

public partial class Reward
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("XP")]
    public long? Xp { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("XPMultiplier")]
    public long? XpMultiplier { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("MonkeyMoney")]
    public long? MonkeyMoney { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("MonkeyMoneyMultiplier")]
    public long? MonkeyMoneyMultiplier { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TokensEasy")]
    public long? TokensEasy { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TokensMedium")]
    public long? TokensMedium { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TokensHard")]
    public long? TokensHard { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TokensImpoppable")]
    public long? TokensImpoppable { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Tokens")]
    public long? Tokens { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("WaterToCrucible")]
    public bool? WaterToCrucible { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CrucibleLimit")]
    public long? CrucibleLimit { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CrucibleRangeCheck")]
    public long? CrucibleRangeCheck { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("LockedTowers")]
    public string[] LockedTowers { get; set; }
}