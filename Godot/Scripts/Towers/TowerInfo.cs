using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public class TowerInfo
{
    public static readonly TowerInfo InvalidTower = new("invalid");

    public string FactoryName => TypeName;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("AircraftList")]
    public string[] AircraftList { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("BaseCost")]
    public long? BaseCost { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CanBePlacedInWater")]
    public bool? CanBePlacedInWater { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CanBePlacedOnLand")]
    public bool? CanBePlacedOnLand { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CanBePlacedOnPath")]
    public bool? CanBePlacedOnPath { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("CanTargetCamo")]
    public bool? CanTargetCamo { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("DefaultWeapons")]
    public string[] DefaultWeapons { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("ActiveWeaponSlots")]
    public bool[] ActiveWeaponSlots { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Icon")]
    public string Icon { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("PlacementH")]
    public float? PlacementH { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("PlacementRadius")]
    public float? PlacementRadius { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("PlacementW")]
    public float? PlacementW { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("RankToUnlock")]
    public long? RankToUnlock { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("RotatesToTarget")]
    public bool? RotatesToTarget { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TargetIsWeaponOrigin")]
    public bool? TargetIsWeaponOrigin { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TargetingMode")]
    public string TargetingMode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TargetsManually")]
    public bool? TargetsManually { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("TypeName")]
    public string TypeName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("Upgrades")]
    public string[][] Upgrades { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("UpgradeGateway")]
    public UpgradeGateway[][] UpgradeGateway { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("UpgradePrices")]
    public long[][] UpgradePrices { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("UpgradeIcons")]
    public string[][] UpgradeIcons { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("UpgradeAvatars")]
    public string[][] UpgradeAvatars { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("UseRadiusPlacement")]
    public bool? UseRadiusPlacement { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("SpriteUpgradeDefinition")]
    public string SpriteUpgradeDefinition { get; set; }

    private TowerUpgradeSprites? _towerSprites;

    public TowerInfo() : base()
    {
        TypeName = "invalid";
        
        AircraftList = Array.Empty<string>();
        DefaultWeapons = Array.Empty<string>();
        ActiveWeaponSlots = Array.Empty<bool>();
        Icon = string.Empty;
        TargetingMode = string.Empty;
        Upgrades = Array.Empty<string[]>();
        UpgradeGateway = Array.Empty<UpgradeGateway[]>();
        UpgradePrices = Array.Empty<long[]>();
        UpgradeIcons = Array.Empty<string[]>();
        UpgradeAvatars = Array.Empty<string[]>();
        SpriteUpgradeDefinition = string.Empty;
    }

    public TowerInfo(string factoryName) : base()
    {
        TypeName = factoryName;

        AircraftList = Array.Empty<string>();
        DefaultWeapons = Array.Empty<string>();
        ActiveWeaponSlots = Array.Empty<bool>();
        Icon = string.Empty;
        TargetingMode = string.Empty;
        Upgrades = Array.Empty<string[]>();
        UpgradeGateway = Array.Empty<UpgradeGateway[]>();
        UpgradePrices = Array.Empty<long[]>();
        UpgradeIcons = Array.Empty<string[]>();
        UpgradeAvatars = Array.Empty<string[]>();
        SpriteUpgradeDefinition = string.Empty;
    }

    public TowerUpgradeSprites GetSprites()
    {
        if (_towerSprites != null) return _towerSprites;
        
        const string definitions = "Assets/JSON/TowerSpriteUpgradeDefinitions/";
        var definitionPath = definitions + SpriteUpgradeDefinition;
        var jsonElem = JetFileImporter.Instance().GetJsonParsed(definitionPath);
        _towerSprites = TowerUpgradeSprites.FromJson(jsonElem);

        return _towerSprites;
    }

    public static TowerInfo FromJson(JsonElement element) => element.Deserialize<TowerInfo>() ?? InvalidTower;
}