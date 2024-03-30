namespace BloonsTD5Rewritten.Godot.Scripts.GameMode;

public partial class Reward
{
    public long? Xp { get; set; }
    public long? XpMultiplier { get; set; }
    public long? MonkeyMoney { get; set; }
    public long? MonkeyMoneyMultiplier { get; set; }
    public long? TokensEasy { get; set; }
    public long? TokensMedium { get; set; }
    public long? TokensHard { get; set; }
    public long? TokensImpoppable { get; set; }
    public long? Tokens { get; set; }
    public bool? WaterToCrucible { get; set; }
    public long? CrucibleLimit { get; set; }
    public long? CrucibleRangeCheck { get; set; }
    public string[] LockedTowers { get; set; }
}