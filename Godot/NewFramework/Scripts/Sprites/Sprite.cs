using System;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;

public partial class Sprite : Sprite2D
{
	[Export] public string SpriteName = "";
	[Export] public string TextureName = "";

	private Color _color = Colors.White;
	[Export] public Color Color
	{
		get => _color;
		set
		{
			var mat = Material as ShaderMaterial;
			mat?.SetShaderParameter("color", value);
			_color = value;
		}
	}

	private float _alpha = 1.0f;
	[Export] public float Alpha
	{
		get => _alpha;
		set
		{
			var mat = Material as ShaderMaterial;
			mat?.SetShaderParameter("alpha", value);
			_alpha = value;
		}
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

		//Update the color in the shader in case it wasn't set up yet
		Color = _color;
		Alpha = _alpha;
		Scale *= Cell?.GetQualityScale() ?? 1.0f;
		
		SpriteReady?.Invoke(this, this);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Texture ??= Cell?.GetTexture();
	}
}