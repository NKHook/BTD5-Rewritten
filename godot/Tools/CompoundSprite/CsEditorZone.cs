using System.Collections.Generic;
using System.IO;
using System.Text;
using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;
using Godot;
using BloonsTD5Rewritten.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Tools.CompoundSprite;

public partial class CsEditorZone : HSplitContainer, IFileImporter
{
	[Export] public Node2D? PreviewOwner;
	[Export] public CompoundSprite? PreviewSprite;
	[Export] public SubViewport? PreviewViewport;
	[Export] public ReloadButton? ReloadButton;

	public string CurrentDirectory = string.Empty;
	public Dictionary<uint, byte[]> SpriteFileData = new();

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (PreviewSprite == null)
			return;
		var resolution = PreviewViewport?.Size ?? Vector2.Zero;
		//PreviewSprite!.Position = resolution * 0.5f;
	}

	public void OpenSpriteFile(string path)
	{
		GD.Print("Opening sprite file: " + path);
		ReloadButton!.CurrentFile = path;
		CurrentDirectory = Path.GetDirectoryName(path) ?? string.Empty;
		for (var i = 0; i < PreviewOwner?.GetChildCount(); i++)
		{
			PreviewOwner.GetChild(i).QueueFree();
		}
		Callable.From(() =>
		{
			var sprite = new CompoundSprite();
			sprite.SpriteDefinitionRes = path;
			sprite.DataSource = this;
			sprite.Position = Vector2.Zero;//Vector2.One * 640.0f * 0.5f;
			PreviewOwner?.AddChild(sprite);
			PreviewSprite = sprite;
		}).CallDeferred();
	}

	public void OverrideFileContent(string path, byte[] content)
	{
		SpriteFileData[path.Hash()] = content;
	}
	
	public byte[] GetFileContent(string path)
	{
		if (SpriteFileData.ContainsKey(path.Hash()))
		{
			return SpriteFileData[path.Hash()];
		}
		if (Path.IsPathRooted(path))
		{
			return File.ReadAllBytes(path);
		}
		return File.ReadAllBytes(CurrentDirectory + '/' + path);
	}

	public string GetFileText(string path)
	{
		var data = GetFileContent(path);
		return Encoding.ASCII.GetString(data);
	}

	public Variant GetJsonEntry(string path)
	{
		var data = GetFileText(path);
		return Json.ParseString(data);
	}
	
	public JsonWrapper GetJsonParsed(string path)
	{
		return new JsonWrapper(GetJsonEntry(path));
	}
}
