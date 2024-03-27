using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.Screens;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class TowerManager : ObjectManager<BaseTower>
{
	private BaseTower? _floatingTower;
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
				if (_trackPlacement && _floatingTower != null)
				{
					_floatingTower.QueueFree();
					_floatingTower = null;
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
		
		if (_floatingTower == null) return;

		_floatingTower.Position = _camera?.GetLocalMousePosition() ?? Vector2.Zero;
		if (_floatingTower.Selected) return;
		
		foreach (var selected in Objects.Where(obj => obj.Selected))
		{
			selected.Unselect();
		}
		_floatingTower.Select();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		if (!_trackPlacement) return;
		if (_floatingTower == null) return;
		if (@event is not InputEventMouseButton button) return;
		if (!button.Pressed) return;
		
		AddObject(_floatingTower);
		_floatingTower = null;
	}

	
	public void SetFloating(BaseTower? tower)
	{
		_floatingTower = tower;
		_placementLayer?.AddChild(_floatingTower);
	}
}