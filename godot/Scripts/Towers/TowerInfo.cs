using System;
using System.Collections.Generic;
using BloonsTD5Rewritten.Scripts.Weapons;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Towers;

public class TowerInfo
{
    public static readonly TowerInfo InvalidTower = new(TowerType.Invalid);

    public bool IsAgent;
    public TowerType TypeName;
    public bool NewTower;
    public bool HideWhenLocked;
    public bool UnlocksInGame;
    public TargetingMode GroundTargetingMode;
    public FlightMode AirTargetingMode;
    public bool TargetIsWeaponOrigin;
    public float Duration;
    public int DestroyAfterPops;
    public WeaponType[] DefaultWeapons = Array.Empty<WeaponType>();
    public int[] SlotsToFireOnBloonEscape = Array.Empty<int>();
    public int[] SlotsToFireEveryNShots = Array.Empty<int>();
    public int[] SlotsToFireOnDestroyAfterPops = Array.Empty<int>();
    public int NumberOfShotsToFire;
    public bool PlayAllAnimationsOnFire;
    public bool CanSpotForTowers;
    public bool[] ActiveWeaponSlots = Array.Empty<bool>();
    public Vector2[] WeaponOffsets = Array.Empty<Vector2>();
	// ReSharper disable once InconsistentNaming
	public string FE_Avatar = string.Empty;
    public int BaseCost;
    public int RankToUnlock;
    public int PopsToUnlock;
    public string[][] Upgrades = Array.Empty<string[]>();
    public int[][] UpgradePrices = Array.Empty<int[]>();
    public string[][] UpgradeIcons = Array.Empty<string[]>();
    public string[][] UpgradeAvatars = Array.Empty<string[]>();
    public UpgradeGateway[][] UpgradeGateway = Array.Empty<UpgradeGateway[]>();
    public TowerInfo[] AircraftList = Array.Empty<TowerInfo>();
    public bool CanBePlacedInWater;
    public bool CanBePlacedOnLand;
    public bool CanBePlacedOnPath;
    public bool CanBeOverlapped;
    public bool CanBePlacedMultipleTimes;
    public bool UseRadiusPlacement;
    public float PlacementW;
    public float PlacementH;
    public float PlacementRadius;
    public bool CanTargetCamo;
    public bool RotatesToTarget;
    public bool FireWeaponAndDestroy;
    public bool FiresWeaponsInSequence;
    public int[][] FiringSequence = Array.Empty<int[]>();
    public int[] PlacementFiringSequence = Array.Empty<int>();
    public bool SkipFirstFrameWhenFiring;
    public bool UseDefaultRangeCircle;
    public int DefaultRangeSize;
    public bool DontResetAnimation;
    public int AnimationWeaponSlotIndex;
    public bool DrawWeaponsOnTop;
    public bool ConfirmLevel4FirstUpgrade;
    public float InitialAngle;
    public TowerUpgradeSprites? SpriteUpgradeDefinition;
    public string DefaultSprite = string.Empty;
    
    private readonly List<WeaponInfo?> _defaultWeapons = new();

    public TowerInfo() : base()
    {
        TypeName = TowerType.Invalid;
    }

    public TowerInfo(TowerType typeFlag) : base()
    {
        TypeName = typeFlag;
    }

    public List<WeaponInfo?> GetDefaultWeaponInfo()
    {
        if (_defaultWeapons.Count != 0) return _defaultWeapons;

        const string definitions = "Assets/JSON/WeaponDefinitions/";
        var towerDir = definitions + TypeName + "/";
        foreach (var weaponType in DefaultWeapons)
        {
            if (weaponType == WeaponType.Invalid)
            {
                _defaultWeapons.Add(null);
                continue;
            }

            var info = WeaponFactory.Instance?.GetInfo(TypeName, weaponType);
            _defaultWeapons.Add(info);
        }

        return _defaultWeapons;
    }
    
    public TowerUpgradeSprites? GetSprites() => SpriteUpgradeDefinition;
}