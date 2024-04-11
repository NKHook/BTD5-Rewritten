using BloonsTD5Rewritten.Godot.Screens.Components;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class GameHudScreen : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var pauseButton = GetNode<SpriteButton>("pause_button");
		pauseButton.Pressed += () => ScreenManager.Instance().OpenPopup("InGamePauseScreen");
	}
}