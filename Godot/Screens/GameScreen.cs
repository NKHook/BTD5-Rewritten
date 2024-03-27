using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Screens.Components;
using BloonsTD5Rewritten.Godot.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class GameScreen : BloonsBaseScreen
{
	public static EventHandler? Fabricate;

	public string? MapName = "";
	
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
	}
}