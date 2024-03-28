using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

public partial class BloonFactory : BaseFactory<BloonInfo, Bloon>
{
    public static BloonFactory Instance = null!;
    enum Category
    {
        BloonTypes = 0,
        StatusTypes = 1,
        PropertyNames = 2,
        DrawLayers = 3,
        Abilities = 4,
    }
    
    public BloonFactory() : base(BloonInfo.Invalid)
    {
        Instance = this;
        DefinitionsDir = "Assets/JSON/BloonDefinitions/";

        var bloonTypes = new[]
        {
            "TestBloon",
            "Red",
            "Blue",
            "Green",
            "Yellow",
            "Pink",
            "Black",
            "White",
            "Lead",
            "Zebra",
            "Rainbow",
            "Ceramic",
            "Golden",
            "MOAB",
            "BFB",
            "ZOMG",
            "Bloonarius",
            "Blastapopoulos",
            "Vortex",
            "Dreadbloon"
        };

        var statusTypes = new[]
        {
            "Ice",
            "Glue",
            "GlueOnTrack",
            "Napalm",
            "MoveToPath",
            "Stun",
            "CrippleMOAB",
            "ViralFrost",
            "IceShards",
            "Regen",
            "Camo",
            "MultiLayerDamage",
            "Permafrost",
            "Slow",
            "Sabotage",
            "SignalFlare",
            "BeeTarget",
            "BeeSting",
            "AbsoluteZero",
            "AbsoluteZeroPermafrost",
            "Foam",
            "ShredBloon",
            "MoveOnCurve",
            "DazeEffect",
            "VacStatus",
            "Freeplay",
            "BloonChipperSuck",
            "DamageOverTime",
            "DamageMultiplier"
        };

        var factoryKeys = new[]
        {
            "Type",
            "StatusEffect",
            "StatusImmunity",
            "DamageImmunity"
        };

        var drawLayers = new[]
        {
            "Ground",
            "OverMidlay",
            "Air"
        };

        var abilities = new[]
        {
            "BloonariusAbility",
            "StunTowersAbility",
            "SlowTowersAbility",
            "ShieldAbility"
        };
        
        TypeTracker.LoadCategory(Category.BloonTypes, bloonTypes);
        TypeTracker.LoadCategory(Category.StatusTypes, statusTypes);
        TypeTracker.LoadCategory(Category.PropertyNames, factoryKeys);
        TypeTracker.LoadCategory(Category.DrawLayers, drawLayers);
        TypeTracker.LoadCategory(Category.Abilities, abilities);
    }

    protected override string ToFileName(string factoryName)
    {
        return factoryName + ".bloon";
    }

    protected override BloonInfo GenerateInfo(JsonElement element) => BloonInfo.FromJson(element);
}