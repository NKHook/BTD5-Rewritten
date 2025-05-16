using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Screens.Components;

public partial class TrackThumbnail : Button
{
	[Export] public string TrackName = "";
	private static bool TrackSelected = false;

	public override void _Ready()
	{
		base._Ready();
		
		Debug.Assert(TrackName != "");
		
		Pressed += () =>
		{
			GameScreen.Fabricate += (sender, args) =>
			{
				GameScreen.Fabricate = null;
				if (sender is not GameScreen screen) return;
				// This should be empty at this point
				if (screen.MapName != string.Empty) return;
				
				Debug.Assert(screen.MapName == string.Empty);
				screen.MapName = TrackName;
				TrackSelected = false;
			};
			
			if (TrackSelected) return;
			Scripts.ScreenManager.Instance().SetScreen("GameScreen", true);
			TrackSelected = true;
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