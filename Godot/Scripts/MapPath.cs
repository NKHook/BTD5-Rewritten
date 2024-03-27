using System.Collections.Generic;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts;

public partial class MapPath : Node2D
{
	[Export] public string PathFile = "";

	private static IEnumerable<Path2D> GeneratePaths(JsonElement document)
	{
		var result = new List<Path2D>();
		var i = 0;
		foreach (var nodeObj in document.GetProperty("Nodes").EnumerateArray())
		{
			var path = new Path2D();
			path.Curve = new Curve2D();
			var added = 0;
			foreach (var point in nodeObj.GetProperty("Points").EnumerateArray())
			{
				var x = point[0].GetSingle();
				var y = point[1].GetSingle();

				path.Curve.AddPoint(new Vector2(x, y) * 4.0f);
				added++;
			}
			
			GD.Print("points: " + path.Curve.PointCount + " added: " + added);

			var pathFollow = new PathFollow2D();
			path.AddChild(pathFollow);
			
			var sprite = new Sprite();
			sprite.SpriteName = "rad_egg_01";
			sprite.TextureName = "InGame";
			pathFollow.AddChild(sprite);
			
			path.Name = "path_" + i;
			result.Add(path);
			i++;
		}
		
		return result.ToArray();
	}
	
	public void Initialize()
	{
		var jsonElem = JetFileImporter.Instance().GetJsonParsed(PathFile);
		var paths = GeneratePaths(jsonElem);
		foreach (var path in paths)
		{
			AddChild(path);
		}
	}
	
	public override void _Ready()
	{
		if (PathFile == string.Empty)
			return;
		
		Initialize();
	}
}