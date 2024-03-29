using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class TowerUpgradeSprites
{
    public Dictionary<string, string> Sprites { get; set; }
    
    public Dictionary<string, long> UpgradeLevels { get; set; }

    private readonly Dictionary<string, CompoundSprite> _sprites = new();

    public TowerUpgradeSprites(string spriteFile)
    {
        const string dir = "Assets/JSON/TowerSpriteUpgradeDefinitions/";
        var path = dir + spriteFile;
        var data = JetFileImporter.Instance().GetJsonParsed(path);
        Sprites = data["Sprites"]?.DictAs<string, string>() ?? throw new BTD5WouldCrashException();
        UpgradeLevels = data["UpgradeLevels"]?.DictAs<string, long>() ?? throw new BTD5WouldCrashException();
    }
    
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
    
    public static TowerUpgradeSprites FromJson(JsonWrapper element) => new(element);
}