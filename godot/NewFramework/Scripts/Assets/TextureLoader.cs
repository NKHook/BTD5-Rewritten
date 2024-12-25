using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace BloonsTD5Rewritten.NewFramework.Scripts.Assets;

public partial class TextureLoader : Node
{
	private static TextureLoader? _instance;

	public static TextureLoader? Instance()
	{
		return _instance;
	}
	private Node? _assetImporterConfig;

	private List<SpriteInfo>? _spritesRoot;
	public List<SpriteInfo>? SpritesRoot => _spritesRoot;
	private readonly Dictionary<string, Task<ImageTexture>> _thumbLoadTasks = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug.Assert(_instance == null, "Why are there two texture loaders?");
		_instance = this;
		
		//Get the instance of the gdscript from the root
		_assetImporterConfig = GetNode<Node>("/root/AssetImporterConfig");
		var assetsDir = _assetImporterConfig.Get("assets_dir");
		GD.Print("Loading assets from: " + assetsDir);
		
		var texturesDir = assetsDir + "/Textures";
		var texturesDirAccess = DirAccess.Open(texturesDir);
		_spritesRoot = LoadSpriteInfo(texturesDirAccess);
	}

	public ImageTexture? GetTrackThumb(string trackName)
	{
		if (_thumbLoadTasks.TryGetValue(trackName, out var task))
		{
			return task.IsCompleted ? task.Result : null;
		}

		_thumbLoadTasks[trackName] = Task.Run(() =>
		{
			var assetsDir = _assetImporterConfig?.Get("assets_dir");
			var texturesDir = assetsDir + "/Textures";
			var imageFile = texturesDir + "/Ultra/track_thumbs/" + trackName + "_thumb.jpg";
			var image = Image.LoadFromFile(imageFile);
			var texture = ImageTexture.CreateFromImage(image);
			return texture;
		});
		//Call again to check if it completed
		return GetTrackThumb(trackName);
	}
	
	public SpriteInfo GetSpriteInfo(string name)
	{
		return _spritesRoot!.FirstOrDefault(info => info.Path.EndsWith(name + ".xml"))!;
	}

	public Variant FindCell(string name, string texture)
	{
		var result = _spritesRoot!.Select(info => info.FindCell(name, texture))
			.FirstOrDefault(result => result != null);
		if (result != null)
			return Variant.From(result);

		if (texture == "error")
			throw new Exception("Failed to find error texture...");
		if (texture != "" && FindFrame(texture) == null)
			return FindCell("texture_not_found", "error");
		
		return FindCell("sprite_not_found", "error");
	}
	public Variant FindCell(string name) => FindCell(name, "");

	public FrameInfo? FindFrame(string name)
	{
		return _spritesRoot!.Select(info => info.FindFrame(name)).FirstOrDefault(result => result != null);
	}
		
	private static List<SpriteInfo>? LoadSpriteInfo(DirAccess texturesDir)
	{
		List<SpriteInfo>? results = new();
		var dirPath = texturesDir.GetCurrentDir();
		
		texturesDir.ListDirBegin();
		var filename = texturesDir.GetNext();
		while (!string.IsNullOrEmpty(filename))
		{
			if (!texturesDir.CurrentIsDir())
			{
				results.Add(new SpriteInfo(filename.Replace(".xml", ""), dirPath, dirPath + "/" + filename));
			}
			filename = texturesDir.GetNext();
		}

		return results;
	}
}