using Godot;

namespace BloonsTD5Rewritten.Scripts.MainMenu;

public partial class Building : Node
{
	[Export] public string Screen = "";
	[Export] public string LocName = "";
	[Export] public bool Hovered = false;
}