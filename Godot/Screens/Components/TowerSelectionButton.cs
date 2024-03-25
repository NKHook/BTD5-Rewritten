using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens.Components;

public partial class TowerSelectionButton : SpriteButton
{
	[Export] public string FactoryName;
	[Export] public string TowerIcon = "";
	[Export] public string IconTexture = "in_game_hud";
	[Export] public Vector2 ButtonSize = new Vector2(118, 116);

	public TowerSelectionButton() : base()
	{
		FactoryName = "invalid";
	}
	public TowerSelectionButton(string towerIcon, string factoryName) : base()
	{
		TowerIcon = towerIcon;
		FactoryName = factoryName;
	}
	
	public override void _Ready()
	{
		base._Ready();
		var boxSprite = GetChild<Sprite2D>(0);
		boxSprite.Centered = true;
		boxSprite.Position = ButtonSize * 0.5f;
		
		var icon = new Sprite();
		icon.TextureName = IconTexture;
		icon.SpriteName = TowerIcon;
		icon.SpriteReady += (sender, sprite) =>
		{
			sprite.Centered = true;
			sprite.Position = ButtonSize * 0.5f;
		};
		
		AddChild(icon);
	}
}