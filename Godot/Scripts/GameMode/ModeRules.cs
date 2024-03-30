namespace BloonsTD5Rewritten.Godot.Scripts.GameMode;

public partial class ModeRules
{
    public bool? CanSave { get; set; }
    public bool? PauseBetweenRounds { get; set; }
    public long? BonusTokenRound { get; set; }
    public long? TargetRound { get; set; }
    public bool? DisableHints { get; set; }
    public bool? DisableEveryplay { get; set; }
    public long? BossDamageCash { get; set; }
    public bool? AllowContinueOnGameOver { get; set; }
    public bool? AllowRetryOnGameOver { get; set; }
    public long? MoneyPerRound { get; set; }
    public Reward? Reward { get; set; }
}