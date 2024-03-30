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
		get => _color;
		set
		{
			_colorChanged = true;
			_color = value;
		}
	}

	[Export]
	public float Alpha
	{
		get => _alpha;
		set
		{
			_alphaChanged = true;
			_alpha = value;
		}
	}
	
	public CellEntry? Cell;
	public EventHandler<Sprite>? SpriteReady;
	private Color _color;
	private bool _colorChanged;
	private float _alpha;
	private bool _alphaChanged;
	
	public override void _Ready()
	{
		Cell ??= TextureLoader.Instance()?.FindCell(SpriteName, TextureName).As<CellEntry>();
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

		if (_colorChanged)
		{
			(Material as ShaderMaterial)?.Shader.Set("color", Color);
			(Material as ShaderMaterial)?.Shader.Set("alpha", Alpha);
			
			_colorChanged = false;
		}
		
		Texture ??= Cell?.GetTexture();
	}
}