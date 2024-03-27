using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts;

public partial class MapMaskNode : Node2D
{
	[Export] public string MaskFile = "";
	[Export] public bool DebugShowMask;

	private MapMask? _maskData;
	private Image? _debugImage;
	private Sprite2D? _debugNode;
	
	public override void _Ready()
	{
		if (MaskFile == string.Empty) return;
		
		Initialize();
	}

	public void Initialize()
	{
		_maskData = MapMask.LoadFromFile(MaskFile);
		_debugImage = _maskData.CreateImageForMask();

		var debugTexture = new ImageTexture();
		debugTexture.SetImage(_debugImage);
		
		var debugSprite = new Sprite2D();
		//Scale up to ultra res
		debugSprite.Scale = Vector2.One * 4;
		debugSprite.Texture = debugTexture;
		_debugNode = debugSprite;
		
		AddChild(debugSprite);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		switch (DebugShowMask)
		{
			case true when _debugNode is { Visible: false }:
				_debugNode.Show();
				break;
			case false when _debugNode != null:
				_debugNode.Hide();
				break;
		}
	}
}