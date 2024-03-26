using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class TowerUpgradeSprites
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Sprites")]
    public Dictionary<string, string>? Sprites { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("UpgradeLevels")]
    public Dictionary<string, long>? UpgradeLevels { get; set; }

    private readonly Dictionary<string, CompoundSprite> _sprites = new();

    private static string UpgradesAsString(int left, int right) => "" + left + "" + right;

    private string GetSpriteNameAtUpgrade(int left, int right)
    {
        var upgradeLevel = UpgradesAsString(left, right);
        var spriteKey = UpgradeLevels?[upgradeLevel];
        if (spriteKey == null) return string.Empty;
        
        var spriteName = Sprites?[spriteKey.ToString()!];
        return spriteName ?? string.Empty;
    }

    public CompoundSprite GetSpriteAtUpgrade(int left, int right)
    {
        const string spritesDir = "Assets/JSON/TowerSprites/";
        var name = GetSpriteNameAtUpgrade(left, right);
        if (_sprites.TryGetValue(name, out var sprite))
            return sprite.Duplicate() as CompoundSprite ?? throw new InvalidOperationException();

        var file = spritesDir + name + ".json";
        var compound = new CompoundSprite();
        compound.SpriteDefinitionRes = file;
        _sprites[name] = compound;
        return GetSpriteAtUpgrade(left, right);
    }
    
    public static TowerUpgradeSprites FromJson(JsonElement element) => element.Deserialize<TowerUpgradeSprites>() ?? new TowerUpgradeSprites();
}