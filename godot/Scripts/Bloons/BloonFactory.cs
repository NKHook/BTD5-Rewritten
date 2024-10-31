using System;
using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Scripts.Bloons.StatusEffects;
using BloonsTD5Rewritten.Scripts.Weapons;

namespace BloonsTD5Rewritten.Scripts.Bloons;

public partial class BloonFactory : BaseFactory<BloonType, BloonInfo, Bloon>
{
    public static BloonFactory Instance = null!;

    public BloonFactory() : base(BloonInfo.Invalid)
    {
        Instance = this;
        DefinitionsDir = "Assets/JSON/BloonDefinitions/";

        var flagpProperties = new[]
        {
            "Type",
            "StatusEffect",
            "StatusImmunity",
            "DamageImmunity"
        };
    }

    protected override string ToFileName(string factoryName)
    {
        return factoryName + ".bloon";
    }

    private Dictionary<BloonType, BloonInfo> _generatedCache = new ();

    public StatusEffect GenerateStatusEffect(JsonWrapper element)
    {
        var statusType = element["Status"]?.GetFlag<StatusFlag>() ?? StatusFlag.None;
        switch (statusType)
        {
            case StatusFlag.Ice:
                break;
            case StatusFlag.Glue:
                break;
            case StatusFlag.GlueOnTrack:
                break;
            case StatusFlag.Napalm:
                break;
            case StatusFlag.MoveToPath:
                break;
            case StatusFlag.Stun:
                break;
            case StatusFlag.CrippleMOAB:
                break;
            case StatusFlag.ViralFrost:
                break;
            case StatusFlag.IceShards:
                break;
            case StatusFlag.Regen:
                break;
            case StatusFlag.Camo:
                break;
            case StatusFlag.MultiLayerDamage:
                break;
            case StatusFlag.Permafrost:
                break;
            case StatusFlag.Slow:
                break;
            case StatusFlag.Sabotage:
                break;
            case StatusFlag.SignalFlare:
                break;
            case StatusFlag.BeeTarget:
                break;
            case StatusFlag.BeeSting:
                break;
            case StatusFlag.AbsoluteZero:
                break;
            case StatusFlag.AbsoluteZeroPermafrost:
                break;
            case StatusFlag.Foam:
                break;
            case StatusFlag.ShredBloon:
            {
                var result = new ShredBloonEffect();
                result.Type = statusType;
                return result;
            }
            case StatusFlag.MoveOnCurve:
                break;
            case StatusFlag.DazeEffect:
                break;
            case StatusFlag.VacStatus:
                break;
            case StatusFlag.Freeplay:
                break;
            case StatusFlag.BloonChipperSuck:
                break;
            case StatusFlag.DamageOverTime:
                break;
            case StatusFlag.DamageMultiplier:
                break;
            case StatusFlag.None:
            default:
                throw new ArgumentOutOfRangeException();
        }

        throw new BTD5WouldCrashException("Invalid type?");
    }
    
    protected override BloonInfo GenerateInfo(JsonWrapper element)
    {
        var info = new BloonInfo();
        info.Type = element["Type"]?.GetFlag<BloonType>() ??
                    throw new BTD5WouldCrashException("'Type' is an invalid BloonType flag");
        if (_generatedCache.TryGetValue(info.Type, out var generateInfo))
            return generateInfo;
        info.DrawLayer = element["DrawLayer"]?.GetFlag<BloonDrawLayer>() ?? BloonDrawLayer.Ground;
        info.InitialHealth = element["InitialHealth"] ?? 1;
        info.SpriteFile = element["SpriteFile"] ?? string.Empty;
        info.BaseSpeed = element["BaseSpeed"] ?? 1;
        info.SpeedMultiplier = element["SpeedMultiplier"] ?? 1;
        info.Rbe = element["RBE"] ?? 1;
        info.ChildBloons = element["ChildBloons"]?.ArrayAs<string>()
                               .Select(file => JetFileImporter.Instance().GetJsonParsed(DefinitionsDir + file))
                               .Select(GenerateInfo)
                               .ToArray() ??
                           Array.Empty<BloonInfo>();
        info.StatusImmunity = element["StatusImmunity"]?.GetFlag<StatusFlag>() ?? StatusFlag.None;
        info.DamageImmunity = element["DamageImmunity"]?.GetFlag<DamageType>() ?? DamageType.None;
        info.CanGoUnderground = element["CanGoUnderground"] ?? true;
        info.RotateToPathDirection = element["RotateToPathDirection"] ?? false;
        info.Scale = element["Scale"] ?? 1.0f;
        info.Radius = element["Radius"] ?? 10;
        info.HitAddon = element["HitAddon"] ?? 0;
        info.BloonAbility = element["BloonAbility"]?.GetFlag<BloonAbilityFlag>() ?? BloonAbilityFlag.None;
        _generatedCache[info.Type] = info;
        return info;
    }

    protected override void InitializeFactory()
    {
        var importer = JetFileImporter.Instance();
        foreach (var type in Enum.GetValues<BloonType>())
        {
            if (type == BloonType.Invalid)
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