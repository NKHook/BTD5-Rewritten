using System;
using System.Collections.Generic;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class BloonsBaseScreen : Node2D
{
	[Export] public Node? SpriteRoot;
	
	private readonly List<CompoundSprite> _spritesToLoad = new();
	private bool _fullyLoaded = false;
	
	public EventHandler? Loaded = null;
	
	public override void _Ready()
	{
		base._Ready();
		foreach (var child in SpriteRoot?.GetChildren() ?? GetChildren())
		{
			if (child is CompoundSprite compound)
				_spritesToLoad.Add(compound);
		}
	}

	public override void _Process(double delta)
	{
		if (_fullyLoaded) return;
		
		_fullyLoaded = _spritesToLoad.TrueForAll(compound => compound.FullyLoaded);
		if (_fullyLoaded)
			Loaded?.Invoke(this, null!);
	}
}