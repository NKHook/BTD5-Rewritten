using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public partial class WeaponFactory : BaseFactory<WeaponInfo, Weapon>
{
    enum Category
    {
        WeaponTypes = 0,
        DamageTypes = 1,
        TaskTypes = 2,
        CollisionTypes = 3,
        TaskUpgradeOperations = 4,
        MovementTypes = 5,
        RenderLayers = 6,
        MaskChangeTypes = 7,
        FireModes = 8
    }
    
    public WeaponFactory() : base(WeaponInfo.Invalid)
    {
        var weaponTypes = new[]
        {
            "TestWeapon",
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
            "RoadSpikes",
            "HeliPilot",
            "ExplodingPineapple",
            "MeerkatSpy",
            "MeerkatSpyPro",
            "MeerkatSpyBlaster",
            "MeerkatSpyProAddBase",
            "PortableLake",
            "PortableLakePro",
            "ReleaseSeaMonster",
            "SeaMonster",
            "Pontoon",
            "PontoonPro",
            "BloonsdayDevice",
            "BloonsdayDevicePro",
            "SuperMonkeyStorm",
            "SuperMonkeyStormPro",
            "Radadactyl",
            "RadadactylPlane",
            "RadadactylPlaneResetPops",
            "RadadactylPlaneRestoreAmmo",
            "RadadactylPro",
            "RadderdactylPlane",
            "RadderdactylPlaneResetPops",
            "RadderdactylPlaneWhirlwind",
            "RadderdactylPlaneRestoreAmmo",
            "WizardLord",
            "WizardLordChainLightning",
            "WizardLordDragonbreath",
            "WizardLordFireball",
            "WizardLordSummonPhoenix",
            "SuperPhoenixPlane",
            "WizardLordWhirlwind",
            "BananaFarmer",
            "BananaFarmerPro",
            "BananaSkinCannon",
            "BananaSkinCannonShutOff",
            "NailGun",
            "Bloonchipper",
            "MonkeySub",
            "GlueOnTrack",
            "SpikeOPult",
            "Juggernaut",
            "TripleDarts",
            "SuperMonkeyFanClub",
            "RingOfFire",
            "BladeMaelstrom",
            "SupplyDrop",
            "GlaiveRiccochet",
            "GlaiveLord",
            "TurboCharge",
            "SupplyDropPlane",
            "DoubleShot",
            "FlashBomb",
            "SabotageSupplyLines",
            "ClusterBombs",
            "MOABMauler",
            "MOABAssassin",
            "DoubleRange",
            "IceShards",
            "ViralFrost",
            "ArcticWind",
            "AbsoluteZero",
            "GlueStrike",
            "AircraftCarrier",
            "GrapeShot",
            "CannonShip",
            "CrowsNest",
            "MOABTakedown",
            "SpectreBomb",
            "SpectreDart",
            "PineapplePresent",
            "GroundZero",
            "MonkeyAceStrafe",
            "SunGod",
            "Temple",
            "RoboMonkeyLeft",
            "RoboMonkeyRight",
            "TechTerrorLeft",
            "TechTerrorRight",
            "BloonAnnihilation",
            "ChainLightning",
            "Whirlwind",
            "Fireball",
            "SeekingBolt",
            "Dragonbreath",
            "SummonPhoenix",
            "PhoenixPlane",
            "CallToArms",
            "MonkeyBank",
            "PopAndAwe",
            "RayOfDoom",
            "HydraRocketPods",
            "BloonAreaDenialSystem",
            "RocketStorm",
            "RocketStormRockets",
            "SpikeStorm",
            "SpikeBall",
            "SpikeMines",
            "MOABSHREDR",
            "DartStorm",
            "EnergyBeacon",
            "TempleSacrifice",
            "templeExplosion1",
            "templeExplosion2",
            "templeExplosion3",
            "templeExplosion4",
            "templeExplosion5",
            "templeFreeze1",
            "templeFreeze2",
            "templeFreeze3",
            "templeFreeze4",
            "templeFreeze5",
            "templeGlue1",
            "templeGlue2",
            "templeGlue3",
            "templeGlue4",
            "templeGlue5",
            "templeSharp1",
            "templeSharp2",
            "templeSharp3",
            "templeSharp4",
            "templeSharp5",
            "templeWind1",
            "templeWind2",
            "templeWind3",
            "templeWind4",
            "templeWind5",
            "MonkeyBuccaneerAircraft",
            "AircraftCarrierStrafe",
            "TribalTurtleCoconut",
            "TribalTurtleSpear",
            "TribalTurtleCoconutPro",
            "TribalTurtleSpearPro",
            "NinjaKiwiPushback",
            "NinjaKiwiPushbackPro",
            "AngrySquirrel",
            "AngrySquirrelAngry",
            "AngrySquirrelPro",
            "AngrySquirrelAngryPro",
            "BeeKeeper",
            "ShadowDouble",
            "BeeKeeperPro",
            "Swarm",
            "BloonberryBush",
            "BloonberryBushPro",
            "BloonberryBushCreepers",
            "MachineGun",
            "RocketArrayFirst",
            "RocketArrayLast",
            "RocketArrayClosest",
            "RocketArrayStrongest",
            "RazorRotors",
            "Downdraft",
            "Redeploy",
            "VitalSupplies",
            "EvilTemple",
            "FanClubDart",
            "SpawnTurret",
            "SpawnTurretTier4",
            "SentryGun",
            "SentryLegs",
            "CleansingFoam",
            "Overclock",
            "SentryGunTier4",
            "BloonTrap",
            "BloonchipperCollect",
            "BloonchipperLead",
            "BloonchipperMoab",
            "BloonchipperSupaVac",
            "MonkeySubBallistic",
            "MonkeySubFirstStrike",
            "MonkeySubPulse",
            "MonkeySubSupport",
            "MonkeySubRadioactive",
            "MonkeySubSubmerge",
            "BloonScrambler",
            "TowerBoost",
            "CrucibleBoost",
            "GenericCrucibleBoost",
            "BloonHuntDrop",
            "AgentDrop",
            "BossBane",
            "BossChill",
            "BossBlast",
            "BossWeaken"
        };
        
        var damageTypes = new[]
        {
            "Piercing",
            "Fire",
            "Explosive",
            "Plasma",
            "Juggernaut",
            "MOABMauler",
            "Ice",
            "Foam"
        };

        var weaponTasks = new[]
        {
            "Projectile",
            "MultiFire",
            "Damage",
            "StatusEffect",
            "RemoveStatusEffect",
            "AreaOfEffect",
            "RandomFire",
            "TimerFire",
            "Effect",
            "TextEffect",
            "BloonSpawnedEvent",
            "RayIntersect",
            "ChainTasks",
            "Collectable",
            "LaunchAircraft",
            "TowerModifier",
            "Harpoon",
            "Sacrifice",
            "AddChildSprite",
            "ChangeTerrain",
            "CreateTower",
            "ResetPopCount",
            "EnableNextWeaponSlot",
            "EnableWeaponSlot",
            "ResetCooldown",
            "RedeployTower",
            "ChangeSubtaskEnabled",
            "CollectCollectables",
            "RestoreAmmo",
            "FireTaskAtLocation",
            "OverclockTask",
            "RBEProjectile",
            "TrapBloon",
            "DamageSpread",
            "TowerBoost"
        };

        var collisionTypes = new[]
        {
            "Once",
            "Continual",
            "None"
        };

        var taskUpgradeOperations = new[]
        {
            "SelectByLocation",
            "SelectByType",
            "Adjust",
            "Replace"
        };

        var movementTypes = new[]
        {
            "Forward",
            "RotateAroundTower",
            "GoToTarget",
            "Target",
            "MotionCurve",
            "ReturnToSender",
            "MoveToTouch",
            "BeeMovement",
            "BeeSwarmMovement"
        };

        var renderLayers = new[]
        {
            "UnderTowers",
            "Ground",
            "FullScreen",
            "Text"
        };

        var maskChangeTypes = new[]
        {
            "BlockTower",
            "PathTower",
            "Water"
        };

        var fireModes = new[]
        {
            "FireOnApply",
            "FireOnPop",
            "FireOnRemove",
            "FireNormal",
            "FireOnCommand",
            "FireOnLastPop",
            "RemoveOnLastApply"
        };
        
        TypeTracker.LoadCategory(Category.WeaponTypes, weaponTypes);
        TypeTracker.LoadCategory(Category.DamageTypes, damageTypes);
        TypeTracker.LoadCategory(Category.TaskTypes, weaponTasks);
        TypeTracker.LoadCategory(Category.CollisionTypes, collisionTypes);
        TypeTracker.LoadCategory(Category.TaskUpgradeOperations, taskUpgradeOperations);
        TypeTracker.LoadCategory(Category.MovementTypes, movementTypes);
        TypeTracker.LoadCategory(Category.RenderLayers, renderLayers);
        TypeTracker.LoadCategory(Category.MaskChangeTypes, maskChangeTypes);
        TypeTracker.LoadCategory(Category.FireModes, fireModes);
    }

    protected override string ToFileName(string factoryName)
    {
        return factoryName + ".weapon";
    }

    protected override WeaponInfo GenerateInfo(JsonElement element)
    {
        return WeaponInfo.FromJson(element);
    }
}