using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;

public partial class Sprite : Sprite2D
{
	[Export] public string SpriteName = "";
	[Export] public string TextureName = "";
	[Export] public Color Color
	{
		get => (Material as ShaderMaterial)?.Shader.Get("color").AsColor() ?? Colors.White;
		set => (Material as ShaderMaterial)?.Shader.Set("color", value);
	}
	[Export] public float Alpha
	{
		get => (Material as ShaderMaterial)?.Shader.Get("alpha").AsSingle() ?? 1.0f;
		set => (Material as ShaderMaterial)?.Shader.Set("alpha", value);
	}
	
	public CellEntry? Cell;
	public EventHandler<Sprite>? SpriteReady;
	
	public override void _Ready()
	{
		Cell = TextureLoader.Instance()?.FindCell(SpriteName, TextureName).As<CellEntry>();
		Texture = Cell?.GetTexture();
		RegionEnabled = true;
		RegionRect = Cell?.GetRegion() ?? new Rect2();
		Material = new ShaderMaterial();
		(Material as ShaderMaterial)!.Shader = GD.Load<VisualShader>("res://Godot/Shaders/compound_sprite.tres");
		SpriteReady?.Invoke(this, this);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Texture ??= Cell?.GetTexture();
	}
}