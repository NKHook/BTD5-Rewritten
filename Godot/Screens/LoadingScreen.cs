using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class LoadingScreen : Node2D
{
	[Export] public string ScreenToLoad = "";

	private Node2D _screenManager;
	public override void _Ready()
	{
		Debug.Assert(ScreenToLoad != "", "Loading screen has no screen to load!");
		_screenManager = FindParent("screen_manager") as Node2D;
		
		var screenPromise = AsyncResourceLoader.Instance()
			.Load<PackedScene>("res://Godot/Screens/" + ScreenToLoad + ".tscn");

		screenPromise.Then += (sender, scene) =>
		{
			//Remove any screens that still havent been removed
			foreach(var screen in _screenManager.GetChildren())
			{
				if (screen != this)
				{
					screen.Free();
				}
			}
			
			//Add the desired scene
			_screenManager.AddChild(scene.Instantiate());
			
			//Remove the loading screen
			Free();
		};
	}

}