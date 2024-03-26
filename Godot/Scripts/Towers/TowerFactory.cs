using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class TowerFactory : BaseFactory<TowerInfo, BaseTower>
{
    public static TowerFactory Instance = null!;
    enum Category
    {
        TowerFactoryNames = 0,
        GroundTargets = 1,
        PropertyNames = 2,
        ArialTargets = 3,
        ProStatuses = 4,
        TowerTypeNames = 5
    }

    public TowerFactory() : base(TowerInfo.InvalidTower)
    {
        Instance = this;
        DefinitionsDir = "Assets/JSON/TowerDefinitions/";

        var towerNames = new[]
        {
            "TestTower",
            "DartMonkey",
            "TackTower",
            "SniperMonkey",
            "BoomerangThrower",
            "NinjaMonkey",
            "BombTower",
            "IceTower",
            "GlueGunner",
            "MonkeyBuccaneer",
            "MonkeyAce",
            "SuperMonkey",
            "MonkeyApprentice",
            "MonkeyVillage",
            "BananaFarm",
            "MortarTower",
            "DartlingGun",
            "SpikeFactory",
            "HeliPilot",
            "RoadSpikes",
            "ExplodingPineapple",
            "MonkeyEngineer",
            "Bloonchipper",
            "MonkeySub",
            "MeerkatSpyPro",
            "MeerkatSpy",
            "TribalTurtlePro",
            "TribalTurtle",
            "PortableLakePro",
            "PortableLake",
            "PontoonPro",
            "Pontoon",
            "BloonsdayDevicePro",
            "BloonsdayDevice",
            "AngrySquirrelPro",
            "AngrySquirrel",
            "SuperMonkeyStormPro",
            "SuperMonkeyStorm",
            "BeeKeeperPro",
            "BeeKeeper",
            "BloonberryBushPro",
            "BloonberryBush",
            "RadadactylPro",
            "Radadactyl",
            "BananaFarmerPro",
            "BananaFarmer",
            "NinjaKiwiPro",
            "NinjaKiwi",
            "WizardLord",
            "AcePlane",
            "AircraftCarrier",
            "PhoenixPlane",
            "SuperPhoenixPlane",
            "SupplyDropPlane",
            "HeliPilotAircraft",
            "RadadactylPlane",
            "RadderdactylPlane",
            "MonkeyEngineerSentry",
            "MonkeyEngineerSentryTier4",
            "GameDummy"
        };
        var groundTargets = new[]
        {
            "First",
            "Last",
            "Close",
            "Strong",
            "AnyInRange",
            "Any",
            "ManualActive",
            "ManualPassive",
            "Arc",
            "None",
            "Submerge"
        };
        var propertyNames = new[]
        {
            "Type"
        };
        var arialTargets = new[]
        {
            "FollowPath",
            "Patrol",
            "FollowPlayer",
            "LockInPlace",
            "Pursuit"
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
        };

        TypeTracker.LoadCategory(Category.TowerFactoryNames, towerNames);
        TypeTracker.LoadCategory(Category.GroundTargets, groundTargets);
        TypeTracker.LoadCategory(Category.PropertyNames, propertyNames);
        TypeTracker.LoadCategory(Category.ArialTargets, arialTargets);
        TypeTracker.LoadCategory(Category.ProStatuses, proStatuses);
        TypeTracker.LoadCategory(Category.TowerTypeNames, towerNames);
    }

    protected override string ToFileName(string typeName)
    {
        return typeName + ".tower";
    }

    protected override TowerInfo GenerateInfo(JsonElement element)
    {
        return TowerInfo.FromJson(element);
    }
}