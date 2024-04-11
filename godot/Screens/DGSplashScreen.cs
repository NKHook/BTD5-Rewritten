using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class DGSplashScreen : Node2D
{
	private float _time;
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_time += (float)delta;
		if (_time > 1.2f)
		{
			ScreenManager.Instance().SetScreen("MainMenuScreen");
		}
	}
}