using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using Godot;
using CellEntry = BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets.CellEntry;
using TextureLoader = BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets.TextureLoader;

namespace BloonsTD5Rewritten.Godot.Scripts.Debug;

public partial class SheetTester : Sprite2D
{
	private Image _frame = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_frame = TextureLoader.Instance().FindCell("dart_monkey_body", "InGame").As<CellEntry>().GetImage();
		Texture = ImageTexture.CreateFromImage(_frame);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}