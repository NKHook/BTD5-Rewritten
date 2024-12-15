using Godot;
using BloonsTD5Rewritten.NewFramework.Scripts.Compound;

public partial class CsEditorZone : HSplitContainer
{
	[Export] public CompoundSprite? PreviewSprite;
	[Export] public SubViewport? PreviewViewport;

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (PreviewSprite == null)
			return;
		var resolution = PreviewViewport?.Size ?? Vector2.Zero;
		PreviewSprite!.Position = resolution * 0.5f;
	}
}
