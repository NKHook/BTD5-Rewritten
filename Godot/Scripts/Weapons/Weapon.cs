namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public class Weapon
{
    private WeaponInfo _definition;
    public Weapon(WeaponInfo definition)
    {
        _definition = definition;
    }

    public float Range => (float)_definition.TargetRange.GetValueOrDefault(0.0);
}