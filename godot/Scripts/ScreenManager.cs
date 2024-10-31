using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Screens;
using Godot;

namespace BloonsTD5Rewritten.Scripts;

public partial class ScreenManager : Node2D
{
	private static ScreenManager _instance = null!;
	public static ScreenManager Instance() => _instance;

	[Export] private string _startingScreen = "DGSplashScreen";
	private PackedScene? _loadingScreenScene;
	public Node2D? CurrentScreen { get; private set; }

	private readonly Stack<Node2D?> _popups = new();
	public override void _Ready()
	{
		_loadingScreenScene = GD.Load<PackedScene>("res://Screens/LoadingScreen.tscn");
		_instance = this;
		
		SetScreen(_startingScreen);
	}

	public void SetScreen(string screenName, bool loadingScreen = false)
	{
		ClearPopups();

		if (loadingScreen)
		{
			var loadingScreenInst = _loadingScreenScene?.Instantiate<LoadingScreen>();
			Debug.Assert(loadingScreenInst != null);
			loadingScreenInst!.ScreenToLoad = screenName;
			
			AddChild(loadingScreenInst);
			loadingScreenInst.ScreenLoaded += (sender, screen) => SetScreenNode(screen);
		}
		else
		{
			var promise = AsyncResourceLoader.Instance().Load<PackedScene>("res://Screens/" + screenName + ".tscn");
			if (promise != null) promise.Then += (sender, scene) => SetScreenNode(scene.Instantiate<Node2D>());
		}
	}

	public void OpenPopup(string popupName)
	{
		if (FindChild(popupName) != null) return;
		
		var promise = AsyncResourceLoader.Instance().Load<PackedScene>("res://Screens/" + popupName + ".tscn");
		if (promise != null)
			promise.Then += (sender, popup) =>
			{
				var popupScreen = popup.Instantiate();
				AddChild(popupScreen);
				var top = _popups.Count > 0 ? _popups.First() : null;
				if (top != null) top.Visible = false;

				_popups.Push(popupScreen as Node2D);
			};
	}

	public void ClosePopup()
	{
		_popups.Pop()?.QueueFree();
		
		var top = _popups.FirstOrDefault();
		if (top != null) top.Visible = true;
	}

	private void ClearPopups()
	{
		foreach (var popup in _popups)
		{
			popup?.QueueFree();
		}
		_popups.Clear();
	}

	private void SetScreenNode(Node2D screen)
	{
		if (CurrentScreen != null)
			CallDeferred("remove_child", CurrentScreen);
		CurrentScreen?.QueueFree();
		
		CallDeferred("add_child", screen);
		CurrentScreen = screen;
	}
}