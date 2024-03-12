using System;
using System.IO;
using System.Linq;
using System.Text;
using BloonsTD5Custom.Godot.Scripts;
using Godot;
using Ionic.Zip;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public partial class JetFileImporter : Node
{
	private Node _assetImporterConfig = null;
	private ZipFile _jetFile = null;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_assetImporterConfig = GetNode<Node>("/root/AssetImporterConfig");
		var jetFile = _assetImporterConfig.Get("jet_file").ToString();
		
		var zipFile = new ZipFile(jetFile);
		zipFile.Password = "Q%_{6#Px]]";
		_jetFile = zipFile;

		const string testPath = "Assets/JSON/TowerSprites/DartMonkey.json";
		GD.Print("Path: " + testPath);
		var data = GetFileContent(testPath);
		GD.Print(data);
	}

	public ZipEntry GetFileEntry(string path)
	{
		return _jetFile.FirstOrDefault(entry => entry.FileName == path);
	}

	public string GetFileContent(string path)
	{
		var entry = GetFileEntry(path);
		var stream = new MemoryStream();
		entry.Extract(stream);

		stream.Seek(0, SeekOrigin.Begin);
		var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
	
	public Variant GetJsonEntry(string path)
	{
		var data = GetFileContent(path);
		return Json.ParseString(data);
	}
}