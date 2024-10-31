using BloonsTD5Rewritten.Screens.Components;
using BloonsTD5Rewritten.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Screens;

public partial class PopupScreenBase : Node2D
{
	public override void _Ready()
	{
		var closeButton = GetNode<SpriteButton>("popup_layer/close_button");
		//Destroy the popup when the close button is pressed
		closeButton.Pressed += ScreenManager.Instance().ClosePopup;
	}
}