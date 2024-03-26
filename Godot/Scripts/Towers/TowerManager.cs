using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class TowerManager : ObjectManager<BaseTower>
{
	private BaseTower? _floatingTower;
	private Node2D? _placementLayer;
	private Camera2D? _camera;

	public override void _Ready()
	{
		base._Ready();

		_camera = ScreenManager.Instance().CurrentScreen?.GetNode<Camera2D>("main_camera");

		_placementLayer = new Node2D();
		_placementLayer.Name = "placement_layer";
		AddChild(_placementLayer);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		if (_floatingTower == null) return;

		_floatingTower.Position = _camera?.GetLocalMousePosition() ?? Vector2.Zero;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
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