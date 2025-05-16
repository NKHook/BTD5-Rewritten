using System;
using BloonsTD5Rewritten.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Scripts.Level;

namespace BloonsTD5Rewritten.Screens;

public partial class GameScreen : BloonsBaseScreen
{
	public static EventHandler? Fabricate;

	public string MapName = "";
	
	public override void _Ready()
	{
		base._Ready();
		
		Fabricate?.Invoke(this, null!);

		var mapDir = "Assets/JSON/LevelDefinitions/" + MapName + "/";
		
		var props = GetNode<CompoundSprite>("map_props");
		props.SpriteDefinitionRes = mapDir + MapName + ".props";
		props.Initialize();

		var mask = GetNode<MapMaskNode>("map_mask");
		mask.MaskFile = mapDir + MapName + ".mask";
		mask.Initialize();

		var path = GetNode<MapPath>("map_path");
		path.PathFile = mapDir + MapName + ".path";
		path.Initialize();
	}
}