using Godot;
using System;

public partial class ZoomSlider : VSlider
{
	[Export] public Camera2D? PreviewCamera;

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ZoomIn"))
		{
			Value += Input.GetActionStrength("ZoomIn");
			GD.Print("ZoomIn");
		}

		if (Input.IsActionPressed("ZoomOut"))
		{
			Value -= Input.GetActionStrength("ZoomOut");
			GD.Print("ZoomOut");
		}
		
		PreviewCamera!.Zoom = Vector2.One * (float)Value;
	}
}
