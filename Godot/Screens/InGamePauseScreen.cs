using Godot;
using System;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Screens.Components;

public partial class InGamePauseScreen : PopupScreenBase
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		var homeButton = GetNode<SpriteButton>("popup_layer/home_button");
		homeButton.Pressed += () => ScreenManager.Instance().SetScreen("MainMenuScreen", true);
	}
}
