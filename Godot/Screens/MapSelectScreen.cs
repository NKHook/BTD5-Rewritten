using BloonsTD5Rewritten.Godot.Screens.Components;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class MapSelectScreen : BloonsBaseScreen
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		var backButton = GetNode<SpriteButton>("back_button");
		backButton.Pressed += () =>
		{
			ScreenManager.Instance().SetScreen("MainMenuScreen", true);
		};
	}
}