using System;
using System.Linq;
using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Scripts.Weapons;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Towers;

public partial class TowerFactory : BaseFactory<TowerType, TowerInfo, BaseTower>
{
    public static TowerFactory Instance = null!;

    public TowerFactory() : base(TowerInfo.InvalidTower)
    {
        Instance = this;
        DefinitionsDir = "Assets/JSON/TowerDefinitions/";

        /*var towerNames = Enum.GetNames<TowerType>();
        var flagProperties = new[]
        {
            "Type"
        };
        var proStatuses = new[]
        {
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NonPro",
            "Pro",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
            "NA",
        };*/
    }

    protected override string ToFileName(string typeName)
    {
        return typeName + ".tower";
    }

    protected override TowerInfo GenerateInfo(JsonWrapper element)
    {
        var info = new TowerInfo();
        if (element.TryGetProperty("IsAircraft", out var isAircraft) && isAircraft == true)
        {
            //Blah
        }

        info.IsAgent = element["IsAgent"] ?? false;
        info.TypeName = element["TypeName"]?.GetFlag<TowerType>() ?? TowerType.Invalid;
        info.NewTower = element["NewTower"] ?? false;
        info.HideWhenLocked = element["HideWhenLocked"] ?? false;
        info.UnlocksInGame = element["UnlocksInGame"] ?? true;
        info.GroundTargetingMode = element["TargetingMode"]?.GetFlag<TargetingMode>() ?? TargetingMode.Any;
        info.TargetIsWeaponOrigin = element["TargetIsWeaponOrigin"] ?? false;
        info.Duration = element["Duration"] ?? 0.0f;
        info.DestroyAfterPops = element["DestroyAfterPops"] ?? 0;
        info.DefaultWeapons = element["DefaultWeapons"]?
            .ArrayAs<string>()
            .Select(file => file.Length > 5 ? file[..^7] : "")
            .Select(name => WeaponFactory.Instance!.StringToFlag<WeaponType>(name))
            .ToArray() ?? Array.Empty<WeaponType>();
        info.SlotsToFireOnBloonEscape = element["SlotsToFireOnBloonEscape"]?.ArrayAs<int>() ?? Array.Empty<int>();
        info.SlotsToFireEveryNShots = element["SlotsToFireEveryNShots"]?.ArrayAs<int>() ?? Array.Empty<int>();
        info.SlotsToFireOnDestroyAfterPops =
            element["SlotsToFireOnDestroyAfterPops"]?.ArrayAs<int>() ?? Array.Empty<int>();
        info.NumberOfShotsToFire = element["NumberOfShotsToFire"] ?? 0;
        info.PlayAllAnimationsOnFire = element["PlayAllAnimationsOnFire"] ?? false;
        info.CanSpotForTowers = element["CanSpotForTowers"] ?? false;
        info.ActiveWeaponSlots = element["ActiveWeaponSlots"]?.ArrayAs<bool>() ?? Array.Empty<bool>();
        info.WeaponOffsets = element["WeaponOffsets"]?.ArrayAs<Vector2>() ?? Array.Empty<Vector2>();
        info.FE_Avatar = element["FE_Avatar"] ?? string.Empty;
        info.BaseCost = element["BaseCost"] ?? 0;
        info.RankToUnlock = element["RankToUnlock"] ?? 0;
        info.PopsToUnlock = element["PopsToUnlock"] ?? 0;
        info.Upgrades = element["Upgrades"]?.ArrayAs<string[]>() ?? Array.Empty<string[]>();
        info.UpgradePrices = element["UpgradePrices"]?.ArrayAs<int[]>() ?? Array.Empty<int[]>();
        info.UpgradeIcons = element["UpgradeIcons"]?.ArrayAs<string[]>() ?? Array.Empty<string[]>();
        info.UpgradeAvatars = element["UpgradeAvatars"]?.ArrayAs<string[]>() ?? Array.Empty<string[]>();
        info.UpgradeGateway = element["UpgradeGateway"]?
            .EnumerateArray()
            .Select(arr => arr
                .EnumerateArray()
                .Select(
                    gateway => new UpgradeGateway
                    {
                        Rank = gateway["Rank"] ?? 0,
                        Xp = gateway["XP"] ?? 0
                    }).ToArray()
            ).ToArray() ?? Array.Empty<UpgradeGateway[]>();
        info.AircraftList = element["AircraftList"]?
            .ArrayAs<string>()
            .Select(file => JetFileImporter.Instance().GetJsonParsed(DefinitionsDir + file))
            .Select(GenerateInfo)
            .ToArray() ?? Array.Empty<TowerInfo>();
        info.CanBePlacedInWater = element["CanBePlacedInWater"] ?? false;
        info.CanBePlacedOnLand = element["CanBePlacedOnLand"] ?? true;
        info.CanBePlacedOnPath = element["CanBePlacedOnPath"] ?? false;
        info.CanBeOverlapped = element["CanBeOverlapped"] ?? false;
        info.CanBePlacedMultipleTimes = element["CanBePlacedMultipleTimes"] ?? true;
        info.UseRadiusPlacement = element["UseRadiusPlacement"] ?? true;
        info.PlacementW = element["PlacementW"] ?? 0.0f;
        info.PlacementH = element["PlacementH"] ?? 0.0f;
        info.PlacementRadius = element["PlacementRadius"] ?? 64.0f;
        info.CanTargetCamo = element["CanTargetCamo"] ?? false;
        info.RotatesToTarget = element["RotatesToTarget"] ?? true;
        info.FireWeaponAndDestroy = element["FireWeaponAndDestroy"] ?? false;
        info.FiresWeaponsInSequence = element["FiresWeaponsInSequence"] ?? false;
        info.FiringSequence = element["FiringSequence"]?.ArrayAs<int[]>() ?? Array.Empty<int[]>();
        info.PlacementFiringSequence = element["PlacementFiringSequence"]?.ArrayAs<int>() ?? Array.Empty<int>();
        info.SkipFirstFrameWhenFiring = element["SkipFirstFrameWhenFiring"] ?? false;
        info.UseDefaultRangeCircle = element["UseDefaultRangeCircle"] ?? false;
        info.DefaultRangeSize = element["DefaultRangeSize"] ?? 0;
        info.DontResetAnimation = element["DontResetAnimation"] ?? false;
        info.AnimationWeaponSlotIndex = element["AnimationWeaponSlotIndex"] ?? 0;
        info.DrawWeaponsOnTop = element["DrawWeaponsOnTop"] ?? false;
        info.ConfirmLevel4FirstUpgrade = element["ConfirmLevel4FirstUpgrade"] ?? false;
        info.InitialAngle = element["InitialAngle"] ?? 0.0f;
        var upgradeDefinitions = element["SpriteUpgradeDefinition"]?.GetString() ?? string.Empty;
        info.SpriteUpgradeDefinition =
            upgradeDefinitions != string.Empty ? new TowerUpgradeSprites(upgradeDefinitions) : default;
        info.DefaultSprite = element["DefaultSprite"] ?? string.Empty;

        return info;
    }

    protected override void InitializeFactory()
    {
        var importer = JetFileImporter.Instance();
        foreach (var type in Enum.GetValues<TowerType>())
        {
            if (type == TowerType.Invalid)
                continue;
            
            var name = type.ToString();
            var fileName = ToFileName(name);
            var path = DefinitionsDir + fileName;
            var document = importer.GetJsonParsed(path);
            var info = GenerateInfo(document);
            AddInfo(type, info);
        }
    }
}