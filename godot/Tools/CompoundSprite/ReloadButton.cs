using Godot;

namespace BloonsTD5Rewritten.Tools.CompoundSprite;

public partial class ReloadButton : Button
{
	[Export] public CsEditorZone? EditorZone;
	[Export] public SpeedAdjust? SpeedAdjust;
	[Export] public CodeEdit? JsonEdit;
	[Export] public Node2D? PreviewOwner;
	[Export] public string CurrentFile = string.Empty;
	
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		EditorZone.OverrideFileContent(CurrentFile, JsonEdit!.Text.ToAsciiBuffer());
		GD.Print("Reloading!");
		EditorZone?.OpenSpriteFile(CurrentFile);
	}
}