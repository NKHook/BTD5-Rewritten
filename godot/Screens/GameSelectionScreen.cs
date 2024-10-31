using BloonsTD5Rewritten.Screens.Components;

namespace BloonsTD5Rewritten.Screens;

public partial class GameSelectionScreen : PopupScreenBase
{
    public override void _Ready()
    {
        base._Ready();
        
        var newGameButton = GetNode<SpriteButton>("popup_layer/new_game");
        newGameButton.Pressed += () => Scripts.ScreenManager.Instance().SetScreen("MapSelectScreen", true);
    }
}