using BloonsTD5Rewritten.NewFramework.Scripts.Sprites;
using Godot;

namespace BloonsTD5Rewritten.Screens.Components;

public partial class SpriteButton : Button
{
	[Export] public string? SpriteName;
	[Export] public string? TextureName;
	public Sprite? Sprite;
	private bool _wasDisabled = false;
	
	public override void _Ready()
	{
		Sprite = new Sprite();
		Sprite.Centered = false;
		Sprite.SpriteName = SpriteName!;
		Sprite.TextureName = TextureName!;
		Sprite.SpriteReady += (sender, readySprite) =>
		{
			var factor = Size / new Vector2(readySprite.Cell?.W ?? 0, readySprite.Cell?.H ?? 0);
			readySprite.Scale = factor;
		};
		
		AddChild(Sprite);
		MoveChild(Sprite, 0);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Disabled == _wasDisabled) return;
		
		_wasDisabled = Disabled;
		(Sprite?.Material as ShaderMaterial)?.SetShaderParameter("alpha", Disabled ? 0.5f : 1.0f);
	}
}