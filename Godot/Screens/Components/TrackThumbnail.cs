using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens.Components;

public partial class TrackThumbnail : Button
{
	[Export] public string TrackName = "";

	public override void _Ready()
	{
		base._Ready();
		
		Debug.Assert(TrackName != "");
		
		Pressed += () =>
		{
			GameScreen.Fabricate += (sender, args) =>
			{
				GameScreen.Fabricate = null;

				if (sender is GameScreen screen) screen.MapName = TrackName;
			};
			
			ScreenManager.Instance().SetScreen("GameScreen", true);
		};

		var titleLabel = GetNode<Label>("title_plate/title_label");
		titleLabel.Text = TrackName;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Icon = TextureLoader.Instance()?.GetTrackThumb(TrackName);
	}
}