using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.Screens;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class TowerManager : ObjectManager<BaseTower>
{
	public BaseTower? FloatingTower { get; private set; }
	private Node2D? _placementLayer;
	private Camera2D? _camera;
	private Area2D? _mapArea;

	private bool _trackPlacement;
	
	public override void _Ready()
	{
		base._Ready();

		var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
		_camera = gameScreen?.GetNode<Camera2D>("main_camera");
		_mapArea = gameScreen?.GetNode<Area2D>("map_area");

		if (_mapArea != null)
		{
			_mapArea.MouseEntered += () => _trackPlacement = true;
			_mapArea.MouseExited += () =>
			{
				if (_trackPlacement && FloatingTower != null)
				{
					FloatingTower.QueueFree();
					FloatingTower = null;
				}

				_trackPlacement = false;
			};
		}

		_placementLayer = new Node2D();
		_placementLayer.Name = "placement_layer";
		_placementLayer.ZIndex = 48;
		AddChild(_placementLayer);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		if (FloatingTower == null) return;

		FloatingTower.Position = _camera?.GetLocalMousePosition() ?? Vector2.Zero;
		if (FloatingTower.Selected) return;
		
		foreach (var selected in Objects.Where(obj => obj.Selected))
		{
			selected.Unselect();
		}
		FloatingTower.Select();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		if (!_trackPlacement) return;
		if (FloatingTower == null) return;
		if (@event is InputEventMouseMotion)
		{
			FloatingTower.UpdateRadius();
			return;
		}
		if (@event is not InputEventMouseButton button) return;
		if (!button.Pressed) return;
		if (FloatingTower.AtInvalidPosition()) return;
		
		AddObject(FloatingTower);
		FloatingTower = null;
	}

	public bool PlacingTower() => FloatingTower != null;
	
	public void SetFloating(BaseTower? tower)
	{
		FloatingTower = tower;
		FloatingTower?.OwnedBy(this);
		_placementLayer?.AddChild(FloatingTower);
	}
}