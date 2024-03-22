using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Screens.Components;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class GameplayScreen : BloonsBaseScreen
{
	public static EventHandler? Fabricate;

	public string? MapName = "";
	
	public override void _Ready()
	{
		base._Ready();
		
		Fabricate?.Invoke(this, null!);

		var gameHud = GetNode<CanvasLayer>("in_game_hud");
		var pauseButton = gameHud.GetNode<SpriteButton>("pause_button");
		pauseButton.Pressed += () => ScreenManager.Instance().OpenPopup("InGamePauseScreen");
		
		var props = GetNode<CompoundSprite>("map_props");
		props.SpriteDefinitionRes = "Assets/JSON/LevelDefinitions/" + MapName + "/" + MapName + ".props";
		props.Initialize();
	}
}