using BloonsTD5Rewritten.Screens;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Level;

public partial class MapMaskNode : Node2D
{
	[Export] public string MaskFile = "";
	[Export] public bool DebugShowMask;

	private Image? _debugImage;
	private Sprite2D? _debugNode;
	private Label? _maskDebugLabel;
	private Camera2D? _camera;

	public MapMask? MaskData { get; private set; }

	public override void _Ready()
	{
		if (MaskFile == string.Empty) return;
		
		Initialize();
	}

	public void Initialize()
	{
		_maskDebugLabel = GetNode<Label>("/root/game_root/debug_overlay/overlay/mask_hover_display");
		
		var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
		_camera = gameScreen?.GetNode<Camera2D>("main_camera");
		
		MaskData = MapMask.LoadFromFile(MaskFile);
		_debugImage = MaskData.CreateImageForMask();

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

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is not InputEventMouseMotion motion) return;
		
		var mousePosMaybe = _camera?.GetLocalMousePosition();
		var mousePosCam = mousePosMaybe.GetValueOrDefault(Vector2.Zero);
		var mousePos = MapToMask(mousePosCam);

		if (_maskDebugLabel == null || !(MaskData?.HasPixelUltra((int)mousePos.X, (int)mousePos.Y) ?? false)) return;
		
		var newText = "(" + mousePos.X + "," + mousePos.Y + ") Mask: ";
		var maskAtLoc = MaskData.GetPixelUltra((int)mousePos.X, (int)mousePos.Y);

		if ((maskAtLoc & MaskBit.BlockTower) != 0)
		{
			newText += "Unplacable ";
		}
		if ((maskAtLoc & MaskBit.PathTower) != 0)
		{
			newText += "Path ";
		}
		if ((maskAtLoc & MaskBit.Water) != 0)
		{
			newText += "Water ";
		}

		_maskDebugLabel.Text = newText;
	}

	public Vector2 MapToMask(Vector2 mapPos)
	{
		var maskPos = mapPos + (MaskData?.Size ?? Vector2.Zero) * 0.5f * 4.0f;
		maskPos -= Position;
		return maskPos;
	}
}