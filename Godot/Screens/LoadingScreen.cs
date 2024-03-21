using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class LoadingScreen : Node2D
{
	[Export] public string ScreenToLoad = "";
	[Export] public CompoundSprite LoadingSprite = null;

	public EventHandler<Node2D> ScreenLoaded = null!;
	private bool _loadComplete = false;
	public override void _Ready()
	{
		base._Ready();
		Debug.Assert(ScreenToLoad != "", "Loading screen has no screen to load!");
		Debug.Assert(LoadingSprite != null, "Loading screen has no loading sprite!");
		LoadingSprite!.Animating = true;
		LoadingSprite.Loop = false;
		
		var screenPromise = AsyncResourceLoader.Instance()
			.Load<PackedScene>("res://Godot/Screens/" + ScreenToLoad + ".tscn");

		screenPromise.Then += (sender, scene) =>
		{
			//Add the desired scene
			var loadedScene = scene.Instantiate() as Node2D;
			ScreenLoaded?.Invoke(this, loadedScene!);
			_loadComplete = true;
		};
		screenPromise.Error += (sender, exception) =>
		{
			_loadComplete = true;
		};
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (LoadingSprite.Time >= 0.5f && !_loadComplete)
		{
			LoadingSprite.Animating = false;
			LoadingSprite.Time = 0.5f;
		}

		if (_loadComplete)
		{
			LoadingSprite.Animating = true;
		}
		
		if (LoadingSprite.Time >= LoadingSprite.Duration)
			QueueFree();
	}
}