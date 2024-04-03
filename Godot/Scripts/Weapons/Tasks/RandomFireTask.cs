using System;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using BloonsTD5Rewritten.Godot.Scripts.Level;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class RandomFireTask : WeaponTask
{
    public float Range;
    public bool FireFullRange;
    public bool OnlyTargetPlacementLocations;
    public TowerType PlacementTowerType;
    
    public override void Execute(Vector2 where, float angle, Bloon? who)
    {
        var randomWhere = Vector2.Zero;
        if (OnlyTargetPlacementLocations)
        {
            randomWhere = FindPlacementPos();
        }

        foreach (var task in Tasks)
        {
            task.Execute(randomWhere, angle, who);
        }
    }

    private Vector2 FindPlacementPos()
    {
        var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var mapMask = gameScreen?.GetNode<MapMaskNode>("map_mask");
        if(mapMask == null)
            return Vector2.Zero;
        
        var x = (Random.Shared.NextSingle() - 0.5f) * (Range * 2.0f);
        var y = (Random.Shared.NextSingle() - 0.5f) * (Range * 2.0f);

        var towerInfo = TowerFactory.Instance.GetInfo(PlacementTowerType);
        return BaseTower.IsInvalidAt(mapMask, towerInfo, new Vector2(x, y), towerInfo.PlacementRadius)
            ? FindPlacementPos()
            : new Vector2(x, y);
    }
}