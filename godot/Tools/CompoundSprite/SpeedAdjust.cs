using Godot;
using System;

public partial class SpeedAdjust : LineEdit
{
	[Export] public CsEditorZone? EditorZone;
	public float AdjustedSpeed = 1.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TextChanged += OnTextChanged;
	}

	private void OnTextChanged(string value)
	{
		if (EditorZone?.PreviewSprite is not null && float.TryParse(value, out var val))
		{
			EditorZone.PreviewSprite.PlaybackSpeed = val;
			AdjustedSpeed = val;
		}
	}
}
