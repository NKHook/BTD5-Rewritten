namespace BloonsTD5Rewritten.Scripts.Bloons.StatusEffects;

public partial class ModifySpeedStatusEffect : StatusEffect
{
    public float SpeedScale;

    public override float GetBloonSpeed(float speed) => speed * SpeedScale;
}