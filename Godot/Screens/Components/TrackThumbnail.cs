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
			GameplayScreen.Fabricate += (sender, args) =>
			{
				GameplayScreen.Fabricate = null;

				if (sender is GameplayScreen screen) screen.MapName = TrackName;
			};
			
			ScreenManager.Instance().SetScreen("GameplayScreen", true);
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