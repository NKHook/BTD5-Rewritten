using BloonsTD5Rewritten.Screens.Components;
using BloonsTD5Rewritten.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Screens;

public partial class MapSelectScreen : BloonsBaseScreen
{
	private MapSelectCarousel? _carousel;
	private SpriteButton? _leftButton;
	private SpriteButton? _rightButton;
	public override void _Ready()
	{
		base._Ready();

		_carousel = GetNode<MapSelectCarousel>("MapSelectCarousel");
		
		var backButton = GetNode<SpriteButton>("back_button");
		backButton.Pressed += () =>
		{
			ScreenManager.Instance().SetScreen("MainMenuScreen", true);
		};

		_leftButton = GetNode<SpriteButton>("left_button");
		_rightButton = GetNode<SpriteButton>("right_button");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_leftButton!.ButtonPressed)
		{
			_carousel!.Scroll += 50.0f;
		}

		if (_rightButton!.ButtonPressed)
		{
			_carousel!.Scroll -= 50.0f;
		}

		

		if (_carousel?.Scroll >= 0.0f)
		{
			_carousel.Scroll = 0.0f;
			_leftButton.Disabled = true;
		}
		else
		{
			_leftButton.Disabled = false;
		}
		
		var width = _carousel?.GetChild<GridContainer>(0).Size.X;
		if (_carousel?.Scroll <= -width)
		{
			_carousel.Scroll = -width ?? 0.0f;
			_rightButton.Disabled = true;
		}
		else
		{
			_rightButton.Disabled = false;
		}
	}
}