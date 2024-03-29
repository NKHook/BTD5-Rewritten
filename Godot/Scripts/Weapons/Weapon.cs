namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public class Weapon
{
    private readonly WeaponInfo _definition;
    public Weapon(WeaponInfo definition)
    {
        _definition = definition;
    }

    public float Range => _definition.TargetRange;
}