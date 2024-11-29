using Godot;

namespace BloonsTD5Rewritten.Tools.CompoundSprite;

public partial class ReloadButton : Button
{
	[Export] public CodeEdit? JsonEdit;
	[Export] public SubViewport? PreviewViewport;
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
		for (var i = 0; i < PreviewViewport?.GetChildCount(); i++)
		{
			PreviewViewport.GetChild(i).QueueFree();
		}
		Callable.From(() =>
		{
			var sprite = new NewFramework.Scripts.Compound.CompoundSprite();
			sprite.SpriteDefinitionRes = tempPath;
			sprite.LoadDefinitionFromJet = false;
			sprite.Position = Vector2.One * 256.0f;
			PreviewViewport?.AddChild(sprite);
		}).CallDeferred();
	}
}