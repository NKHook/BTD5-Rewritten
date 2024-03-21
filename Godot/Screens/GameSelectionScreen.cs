using BloonsTD5Rewritten.Godot.Screens.Components;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class GameSelectionScreen : PopupScreenBase
{
    public override void _Ready()
    {
        base._Ready();
        
        var newGameButton = GetNode<SpriteButton>("popup_layer/new_game");
        newGameButton.Pressed += () => ScreenManager.Instance().SetScreen("MapSelectScreen", true);
    }
}