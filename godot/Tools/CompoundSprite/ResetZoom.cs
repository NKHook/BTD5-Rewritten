using Godot;
using System;

public partial class ResetZoom : TextureButton
{
	[Export] public VSlider? ZoomSlider;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		ZoomSlider!.Value = 1.2f;
	}
}
