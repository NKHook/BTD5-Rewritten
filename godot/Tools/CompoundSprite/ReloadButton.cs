using Godot;

namespace BloonsTD5Rewritten.Tools.CompoundSprite;

public partial class ReloadButton : Button
{
	[Export] public CsEditorZone? EditorZone;
	[Export] public SpeedAdjust? SpeedAdjust;
	[Export] public CodeEdit? JsonEdit;
	[Export] public Node2D? PreviewOwner;
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		var tempPath = "user://compoundtooltemp.json";
		var access = FileAccess.Open(tempPath, FileAccess.ModeFlags.Write);
		if (access == null)
		{
			GD.PrintErr(FileAccess.GetOpenError());
			return;
		}
		access?.StoreString(JsonEdit?.Text);
		access?.Close();
		GD.Print("Reloading!");
		EditorZone?.OpenSpriteFile(tempPath);
	}
}