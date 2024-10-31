using BloonsTD5Rewritten.Scripts.Towers;

namespace BloonsTD5Rewritten.Scripts.Bloons.StatusEffects;

public partial class ShredBloonEffect : StatusEffect
{
    public DamageOverTimeStatusEffect? _damageEffect;

    public override void Apply(BaseTower? tower, Bloon? who)
    {
        base.Apply(tower, who);
        
        _damageEffect?.Apply(tower, who);
        
        AddChild(_damageEffect);
    }
}