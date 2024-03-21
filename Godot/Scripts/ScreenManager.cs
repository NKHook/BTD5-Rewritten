using Godot;
using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;

public partial class ScreenManager : Node2D
{
	private static ScreenManager _instance = null;
	public static ScreenManager Instance() => _instance;
	
	public override void _Ready()
	{
		_instance = this;
	}

	public void SetScreen(string screenName, Node2D? source = null)
	{
		if (source != null)
		{
			source.Free();
		}
		else
		{
			foreach (var child in GetChildren())
			{
				child.Free();
			}
		}

		var promise = AsyncResourceLoader.Instance().Load<PackedScene>("res://Godot/Screens/" + screenName + ".tscn");
		promise.Then += (sender, scene) =>
		{
			AddChild(scene.Instantiate());
		};
	}

	public void OpenPopup(string popupName)
	{
		if (FindChild(popupName) != null) return;
		
		var popupScreen = GD.Load<PackedScene>("res://Godot/Screens/" + popupName + ".tscn");
		AddChild(popupScreen.Instantiate());
	}
}
