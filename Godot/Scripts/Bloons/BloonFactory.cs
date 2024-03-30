using System;
using System.Linq;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.Scripts.Weapons;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

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

    protected override BloonInfo GenerateInfo(JsonWrapper element)
    {
        var info = new BloonInfo();
        info.Type = element["Type"]?.GetFlag<BloonType>() ??
                    throw new BTD5WouldCrashException("'Type' is an invalid BloonType flag");
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
        info.Scale = element["Scale"] ?? 1;
        info.Radius = element["Radius"] ?? 10;
        info.HitAddon = element["HitAddon"] ?? 0;
        info.BloonAbility = element["BloonAbility"]?.GetFlag<BloonAbilityFlag>() ?? BloonAbilityFlag.None;
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