using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens.Components;

public partial class MapSelectCarousel : Node
{
	private PackedScene? _mapButton;
	private GridContainer? _mapsGrid;
	public float Scroll = 0.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mapButton = GD.Load<PackedScene>("res://Godot/Screens/Components/MapSelectButton.tscn");
		_mapsGrid = GetNode<GridContainer>("thumb_container");

		var tracklist = JetFileImporter.Instance().GetJsonParsed("Assets/JSON/tracklist.json");
		var tracks = tracklist.GetProperty("Tracks");
		foreach (var track in tracks.EnumerateArray())
		{
			var trackName = track.EnumerateArray().ElementAt(1).GetString();
			var button = _mapButton.Instantiate<TrackThumbnail>();
			button.Name = "button_for_" + trackName;
			button.TrackName = trackName!;
			_mapsGrid.AddChild(button);
			_mapsGrid.Columns++;
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_mapsGrid == null) return;
		
		var x = Mathf.Lerp(_mapsGrid.Position.X, Scroll, 0.05f);
		_mapsGrid.Position = new Vector2(x, -360.0f);
	}
}