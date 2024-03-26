using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public partial class BaseTower : Node2D
{
	private readonly TowerInfo _definition;
	private readonly TowerUpgradeSprites _sprites;

	private int leftUpgrade = 0;
	private int rightUpgrade = 0;

	public BaseTower(TowerInfo definition) : base()
	{
		_definition = definition;
		_sprites = definition.GetSprites();
	}
	
	public override void _Ready()
	{
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		var sprite = GetNode<CompoundSprite>("tower_sprite");
		sprite?.Free();

		var newSprite = _sprites.GetSpriteAtUpgrade(leftUpgrade, rightUpgrade);
		newSprite.Name = "tower_sprite";
		AddChild(newSprite);
	}
}