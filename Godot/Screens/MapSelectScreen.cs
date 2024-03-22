using BloonsTD5Rewritten.Godot.Screens.Components;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class MapSelectScreen : BloonsBaseScreen
{
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