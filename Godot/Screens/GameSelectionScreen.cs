using BloonsTD5Rewritten.Godot.Screens.Components;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class GameSelectionScreen : PopupScreenBase
{
    public override void _Ready()
    {
        var loadingScreenScene = GD.Load<PackedScene>("res://Godot/Screens/LoadingScreen.tscn");
        var loadingScreenInst = loadingScreenScene.Instantiate<LoadingScreen>();
        loadingScreenInst.ScreenToLoad = "MapSelectScreen";
        
        var screenManager = GetParent<Node2D>();
        var newGameButton = GetNode<SpriteButton>("popup_layer/new_game");
        newGameButton.Pressed += () => screenManager.AddChild(loadingScreenInst);
    }
}