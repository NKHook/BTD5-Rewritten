using BloonsTD5Rewritten.Screens.Components;

namespace BloonsTD5Rewritten.Screens;

public partial class InGamePauseScreen : PopupScreenBase
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		var homeButton = GetNode<SpriteButton>("popup_layer/home_button");
		homeButton.Pressed += () => Scripts.ScreenManager.Instance().SetScreen("MainMenuScreen", true);
	}
}