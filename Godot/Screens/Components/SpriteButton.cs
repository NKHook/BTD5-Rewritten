using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens.Components;

public partial class SpriteButton : Button
{
	[Export] public string SpriteName;
	[Export] public string TextureName;
	
	public override void _Ready()
	{
		var sprite = new Sprite();
		sprite.Centered = false;
		sprite.SpriteName = SpriteName;
		sprite.TextureName = TextureName;
		sprite.SpriteReady += (sender, readySprite) =>
		{
			var factor = Size / new Vector2(readySprite.Cell.W, readySprite.Cell.H);
			readySprite.Scale = factor;
		};
		
		AddChild(sprite);
		MoveChild(sprite, 0);
	}
}