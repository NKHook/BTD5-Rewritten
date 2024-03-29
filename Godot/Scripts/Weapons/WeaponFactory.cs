using System;
using System.Collections.Generic;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.Scripts.Towers;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public partial class WeaponFactory : BaseFactory<WeaponType, WeaponInfo, Weapon>
{
    public static WeaponFactory? Instance { get; private set; }
    private Dictionary<TowerType, Dictionary<WeaponType, WeaponInfo>> _factoryData = new();
    
    public WeaponFactory() : base(WeaponInfo.Invalid)
    {
        Instance = this;
        DefinitionsDir = "Assets/JSON/WeaponDefinitions/";
    }

    public override WeaponInfo GetInfo(string factoryName)
    {
        throw new NotImplementedException();
    }

    public void AddInfo(TowerType towerFlag, WeaponType weaponFlag, WeaponInfo info)
    {
        if (_factoryData.TryGetValue(towerFlag, out var infoTable))
        {
            infoTable[weaponFlag] = info;
            _factoryData[towerFlag] = infoTable;
        }

        var table = new Dictionary<WeaponType, WeaponInfo>
        {
            [weaponFlag] = info
        };
        _factoryData[towerFlag] = table;
    }

    public WeaponInfo GetInfo(string towerName, string weaponName) =>
        GetInfo(TowerFactory.Instance.StringToFlag<TowerType>(towerName), StringToFlag<WeaponType>(weaponName));
    public WeaponInfo GetInfo(TowerType towerFlag, WeaponType weaponFlag)
    {
        if (_factoryData.TryGetValue(towerFlag, out var infoTable))
        {
            if (infoTable.TryGetValue(weaponFlag, out var weaponInfo))
            {
                return weaponInfo;
            }
        }
        var towerName = TowerFactory.Instance.FlagToString(towerFlag);
        var weaponName = FlagToString(weaponFlag);

        var importer = JetFileImporter.Instance();
        var document = importer.GetJsonParsed(DefinitionsDir + towerName + "/" + ToFileName(weaponName));
        var info = GenerateInfo(document);
        AddInfo(towerFlag, weaponFlag, info);
        return _factoryData[towerFlag][weaponFlag];
    }

    protected override void InitializeFactory()
    {
    }

    protected override string ToFileName(string factoryName)
    {
        return factoryName + ".weapon";
    }

    protected override WeaponInfo GenerateInfo(JsonWrapper element)
    {
        return WeaponInfo.FromJson(element);
    }
    
    
    public override Weapon? Instantiate(string factoryName)
    {
        throw new NotImplementedException();
    }
    public override Weapon? Instantiate(WeaponType flag)
    {
        throw new NotImplementedException();
    }
}