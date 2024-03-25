using System;
using System.Linq;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.Screens.Components;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class InGameTowerSelectionScreen : Node
{
	[Export] public PackedScene? TowerSelectionButtonScene;
	public override void _Ready()
	{
		var rightOrder = JetFileImporter.Instance()
			.GetJsonParsed("Assets/JSON/ScreenDefinitions/TowerSelectionScreen/TowerOrderTop.json");
		var leftOrder = JetFileImporter.Instance()
			.GetJsonParsed("Assets/JSON/ScreenDefinitions/TowerSelectionScreen/TowerOrderBottom.json");

		var rightItems = rightOrder.GetProperty("Items");
		var leftItems = leftOrder.GetProperty("Items");

		var selectionGrid = GetNode<GridContainer>("tower_selection_scroll/tower_selection_grid");
		var biggest = Math.Max(leftItems.EnumerateArray().Count(), rightItems.EnumerateArray().Count());
		selectionGrid.CustomMinimumSize = new Vector2(selectionGrid.CustomMinimumSize.X, biggest * 120);
		
		var listEnumerable = Enumerable.Zip(rightItems.EnumerateArray(), leftItems.EnumerateArray());
		foreach (var (left, right) in listEnumerable)
		{
			var leftButton = CreateButton(left);
			var rightButton = CreateButton(right);
			
			selectionGrid.AddChild(leftButton);
			selectionGrid.AddChild(rightButton);
		}
	}
	
	private CenterContainer CreateButton(JsonElement item)
	{
		var cell = new CenterContainer();
		cell.Size = new Vector2(120, 120);

		var icon = item.GetProperty("Icon").GetString();
		var factoryName = item.GetProperty("FactoryName").GetString();
		var button = TowerSelectionButtonScene?.Instantiate<TowerSelectionButton>();
		button!.Name = factoryName + "_button";
		button.Icon = icon!;
		button.FactoryName = factoryName!;

		cell.AddChild(button);
		return cell;
	}
}