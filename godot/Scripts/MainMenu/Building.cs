using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.MainMenu;

public partial class Building : Node
{
	[Export] public string Screen = "";
	[Export] public string LocName = "";
	[Export] public bool Hovered = false;
}