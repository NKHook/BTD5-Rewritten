using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens.Components;

public partial class TrackThumbnail : Button
{
	[Export] private string _trackName = "";

	public override void _Ready()
	{
		base._Ready();
		
		Debug.Assert(_trackName != "");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Icon = TextureLoader.Instance()?.GetTrackThumb(_trackName);
	}
}