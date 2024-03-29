using System;
using System.Linq;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.Scripts.Weapons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

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
        if (element["IsAircraft"])
        {
        }

        info.IsAgent = element["IsAgent"];
        info.TypeName = element["TypeName"].GetFlag<TowerType>();
        info.NewTower = element["NewTower"];
        info.HideWhenLocked = element["HideWhenLocked"];
        info.UnlocksInGame = element["UnlocksInGame"];
        info.TargetingMode = element["TargetingMode"];
        info.TargetIsWeaponOrigin = element["TargetIsWeaponOrigin"];
        info.Duration = element["Duration"];
        info.DestroyAfterPops = element["DestroyAfterPops"];
        info.DefaultWeapons = element["DefaultWeapons"]
            .ArrayAs<string>()
            .Select(file => file[..^5])
            .Select(name => WeaponFactory.Instance!.StringToFlag<WeaponType>(name))
            .ToArray();
        info.SlotsToFireOnBloonEscape = element["SlotsToFireOnBloonEscape"].ArrayAs<int>();
        info.SlotsToFireEveryNShots = element["SlotsToFireEveryNShots"].ArrayAs<int>();
        info.SlotsToFireOnDestroyAfterPops = element["SlotsToFireOnDestroyAfterPops"].ArrayAs<int>();
        info.NumberOfShotsToFire = element["NumberOfShotsToFire"];
        info.PlayAllAnimationsOnFire = element["PlayAllAnimationsOnFire"];
        info.CanSpotForTowers = element["CanSpotForTowers"];
        info.ActiveWeaponSlots = element["ActiveWeaponSlots"].ArrayAs<bool>();
        info.WeaponOffsets = element["WeaponOffsets"].ArrayAs<Vector2>();
        info.FE_Avatar = element["FE_Avatar"];
        info.BaseCost = element["BaseCost"];
        info.RankToUnlock = element["RankToUnlock"];
        info.PopsToUnlock = element["PopsToUnlock"];
        info.Upgrades = element["Upgrades"].ArrayAs<string[]>();
        info.UpgradePrices = element["UpgradePrices"].ArrayAs<int[]>();
        info.UpgradeIcons = element["UpgradeIcons"].ArrayAs<string[]>();
        info.UpgradeAvatars = element["UpgradeAvatars"].ArrayAs<string[]>();
        info.UpgradeGateway = element["UpgradeGateway"]
            .EnumerateArray()
            .Select(arr => arr
                .EnumerateArray()
                .Select(
                    gateway =>
                    {
                        var gw = new UpgradeGateway();
                        gw.Rank = gateway["Rank"];
                        gw.Xp = gateway["XP"];
                        return gw;
                    }).ToArray()
            ).ToArray();
        info.AircraftList = element["AircraftList"]
            .ArrayAs<string>()
            .Select(file => JetFileImporter.Instance().GetJsonParsed(DefinitionsDir + file))
            .Select(GenerateInfo)
            .ToArray();
        info.CanBePlacedInWater = element["CanBePlacedInWater"];
        info.CanBePlacedOnLand = element["CanBePlacedOnLand"];
        info.CanBePlacedOnPath = element["CanBePlacedOnPath"];
        info.CanBeOverlapped = element["CanBeOverlapped"];
        info.CanBePlacedMultipleTimes = element["CanBePlacedMultipleTimes"];
        info.UseRadiusPlacement = element["UseRadiusPlacement"];
        info.PlacementW = element["PlacementW"];
        info.PlacementH = element["PlacementH"];
        info.PlacementRadius = element["PlacementRadius"];
        info.CanTargetCamo = element["CanTargetCamo"];
        info.RotatesToTarget = element["RotatesToTarget"];
        info.FireWeaponAndDestroy = element["FireWeaponAndDestroy"];
        info.FiresWeaponsInSequence = element["FiresWeaponsInSequence"];
        info.FiringSequence = element["FiringSequence"].ArrayAs<int[]>();
        info.PlacementFiringSequence = element["PlacementFiringSequence"].ArrayAs<int>();
        info.SkipFirstFrameWhenFiring = element["SkipFirstFrameWhenFiring"];
        info.UseDefaultRangeCircle = element["UseDefaultRangeCircle"];
        info.DefaultRangeSize = element["DefaultRangeSize"];
        info.DontResetAnimation = element["DontResetAnimation"];
        info.AnimationWeaponSlotIndex = element["AnimationWeaponSlotIndex"];
        info.DrawWeaponsOnTop = element["DrawWeaponsOnTop"];
        info.ConfirmLevel4FirstUpgrade = element["ConfirmLevel4FirstUpgrade"];
        info.InitialAngle = element["InitialAngle"];
        info.SpriteUpgradeDefinition = new TowerUpgradeSprites(element["SpriteUpgradeDefinition"]);
        info.DefaultSprite = element["DefaultSprite"];

        return info;
    }

    protected override void InitializeFactory()
    {
        foreach (var type in Enum.GetValues<TowerType>())
        {
            
        }
    }
}