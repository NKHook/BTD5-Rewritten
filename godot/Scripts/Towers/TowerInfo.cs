using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.Scripts.Weapons;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

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
    public WeaponType[] DefaultWeapons;
    public int[] SlotsToFireOnBloonEscape;
    public int[] SlotsToFireEveryNShots;
    public int[] SlotsToFireOnDestroyAfterPops;
    public int NumberOfShotsToFire;
    public bool PlayAllAnimationsOnFire;
    public bool CanSpotForTowers;
    public bool[] ActiveWeaponSlots;
    public Vector2[] WeaponOffsets;
    public string FE_Avatar;
    public int BaseCost;
    public int RankToUnlock;
    public int PopsToUnlock;
    public string[][] Upgrades;
    public int[][] UpgradePrices;
    public string[][] UpgradeIcons;
    public string[][] UpgradeAvatars;
    public UpgradeGateway[][] UpgradeGateway;
    public TowerInfo[] AircraftList;
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
    public int[][] FiringSequence;
    public int[] PlacementFiringSequence;
    public bool SkipFirstFrameWhenFiring;
    public bool UseDefaultRangeCircle;
    public int DefaultRangeSize;
    public bool DontResetAnimation;
    public int AnimationWeaponSlotIndex;
    public bool DrawWeaponsOnTop;
    public bool ConfirmLevel4FirstUpgrade;
    public float InitialAngle;
    public TowerUpgradeSprites? SpriteUpgradeDefinition;
    public string DefaultSprite;
    
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