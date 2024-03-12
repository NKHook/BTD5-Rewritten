using System.Collections.Generic;
using System.Linq;
using BloonsTD5Custom.Godot.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public partial class TextureLoader : Node
{
	private static TextureLoader _instance = null;

	public static TextureLoader Instance()
	{
		return _instance;
	}
	private Node _assetImporterConfig = null;

	private List<SpriteInfo> _spritesRoot = null;
	
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

	public SpriteInfo GetSpriteInfo(string name)
	{
		return _spritesRoot.FirstOrDefault(info => info.Path.EndsWith(name + ".xml"));
	}

	public Variant FindCell(string name, string texture)
	{
		return Variant.From(_spritesRoot.Select(info => info.FindCell(name, texture)).FirstOrDefault(result => result != null));
	}
	public Variant FindCell(string name) => FindCell(name, "");

	public FrameInfo FindFrame(string name)
	{
		return _spritesRoot.Select(info => info.FindFrame(name)).FirstOrDefault(result => result != null);
	}
		
	private static List<SpriteInfo> LoadSpriteInfo(DirAccess texturesDir)
	{
		List<SpriteInfo> results = new();
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