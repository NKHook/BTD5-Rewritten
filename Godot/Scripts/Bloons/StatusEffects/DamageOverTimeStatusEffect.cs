namespace BloonsTD5Rewritten.Godot.Scripts.Bloons.StatusEffects;

public partial class DamageOverTimeStatusEffect : StatusEffect
{
    public float DamageRate;
    public float Duration;
    public int NumPersists;
    public int Amount;
    
    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}