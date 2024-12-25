using System.IO;
using System.Linq;
using System.Text;
using Godot;
using Ionic.Zip;

namespace BloonsTD5Rewritten.NewFramework.Scripts.Assets;

public partial class JetFileImporter : Node, IFileImporter
{
	private static JetFileImporter? _instance;
	public static JetFileImporter Instance() => _instance!;
	
	private Node? _assetImporterConfig;
	private ZipFile? _jetFile;
	public ZipFile? JetFile => _jetFile;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_assetImporterConfig = GetNode<Node>("/root/AssetImporterConfig");
		var jetFile = _assetImporterConfig.Get("jet_file").ToString();
		
		var zipFile = new ZipFile(jetFile);
		zipFile.Password = "Q%_{6#Px]]";
		_jetFile = zipFile;

		_instance = this;
	}

	public ZipEntry? GetFileEntry(string path)
	{
		return _jetFile?.Entries.FirstOrDefault(entry => entry.FileName == path);
	}

	public MemoryStream GetFileStream(string path)
	{
		var entry = GetFileEntry(path);
		var stream = new MemoryStream();
		entry?.Extract(stream);
		stream.Seek(0, SeekOrigin.Begin);
		GD.Print("Read file: " + path);
		return stream;
	}

	public byte[] GetFileContent(string path)
	{
		var stream = GetFileStream(path);
		return stream.ToArray();
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
		/*var data = GetFileText(path);

		return new JsonWrapper(JsonSerializer.Deserialize<JsonElement>(data));*/
	}
}
