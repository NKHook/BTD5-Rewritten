using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class LoadingScreen : BloonsBaseScreen
{
	[Export] public string ScreenToLoad = "";
	[Export] public CompoundSprite? LoadingSprite;

	public EventHandler<Node2D> ScreenLoaded = null!;
	private bool _loadComplete;
	private bool _startedLoading;
	private bool _animationStarted;
	public override void _Ready()
	{
		base._Ready();
		
		Debug.Assert(ScreenToLoad != "", "Loading screen has no screen to load!");
		Debug.Assert(LoadingSprite != null, "Loading screen has no loading sprite!");
		LoadingSprite!.Loop = false;

		//Actually start loading the screen once the loading screen itself has loaded
		Loaded += (sender, args) =>
		{
			_animationStarted = true;
			LoadingSprite!.Time = 0.0f;
		};
	}

	private void ActuallyLoadTheScreen()
	{
		var screenPromise = AsyncResourceLoader.Instance()
			.Load<PackedScene>("res://Godot/Screens/" + ScreenToLoad + ".tscn");

		if (screenPromise == null) return;
		
		screenPromise.Then += (_, scene) =>
		{
			//Add the desired scene
			var loadedScene = scene.Instantiate() as Node2D;
			ScreenLoaded?.Invoke(this, loadedScene!);
			if (loadedScene is BloonsBaseScreen baseScreen)
			{
				baseScreen.Loaded += (_, _) => { _loadComplete = true; };
			}
			else
			{
				_loadComplete = true;
			}
		};
		screenPromise.Error += (_, exception) => { _loadComplete = true; };
	}
	
	public override void _Process(double delta)
	{
		base._Process(_startedLoading && !_loadComplete ? 0.0f : delta);
		if (!_animationStarted)
			return;
		
		if (LoadingSprite is { Time: >= 0.5f } && !_loadComplete)
		{
			LoadingSprite.Animating = false;
			LoadingSprite.Time = 0.5f;
			if (!_startedLoading)
			{
				ActuallyLoadTheScreen();
			}
			_startedLoading = true;
		}

		if (_loadComplete)
		{
			LoadingSprite!.Animating = true;
		}
		
		if (LoadingSprite!.Time >= LoadingSprite.Duration)
			QueueFree();
	}
}