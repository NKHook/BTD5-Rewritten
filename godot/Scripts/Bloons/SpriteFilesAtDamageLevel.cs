namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

public partial struct SpriteFilesAtDamageLevel
{
    public long? Damage;
    public string? Sprite;

    public static implicit operator SpriteFilesAtDamageLevel(long damage) =>
        new SpriteFilesAtDamageLevel { Damage = damage };

    public static implicit operator SpriteFilesAtDamageLevel(string sprite) =>
        new SpriteFilesAtDamageLevel { Sprite = sprite };
}