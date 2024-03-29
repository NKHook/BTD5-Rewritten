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
        info.Type = element["Type"].GetFlag<BloonType>();
        info.DrawLayer = element["DrawLayer"].GetFlag<BloonDrawLayer>();
        info.InitialHealth = element["InitialHealth"];
        info.SpriteFile = element["SpriteFile"];
        info.BaseSpeed = element["BaseSpeed"];
        info.SpeedMultiplier = element["SpeedMultiplier"];
        info.Rbe = element["RBE"];
        info.ChildBloons = element["ChildBloons"].ArrayAs<string>()
            .Select(file => JetFileImporter.Instance().GetJsonParsed(file)).Select(GenerateInfo).ToArray();
        info.StatusImmunity = element["StatusImmunity"].GetFlag<StatusFlag>();
        info.DamageImmunity = element["DamageImmunity"].GetFlag<DamageType>();
        info.CanGoUnderground = element["CanGoUnderground"];
        info.RotateToPathDirection = element["RotateToPathDirection"];
        info.Scale = element["Scale"];
        info.Radius = element["Radius"];
        info.HitAddon = element["HitAddon"];
        info.BloonAbility = element["BloonAbility"].GetFlag<BloonAbilityFlag>();
        return info;
    }
    protected override void InitializeFactory()
    {
        
    }
}