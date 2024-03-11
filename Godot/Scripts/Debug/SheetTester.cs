using Godot;
using System;
using BloonsTD5Custom.Godot.NewFramework.Scripts;

public partial class SheetTester : Sprite2D
{
	private Image frame = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		frame = TextureLoader.Instance().FindCell("dart_monkey_body", "InGame").GetImage();
		Texture = ImageTexture.CreateFromImage(frame);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
