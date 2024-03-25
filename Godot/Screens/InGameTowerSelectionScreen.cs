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
	[Export] public PackedScene? TowerSelectionEntryScene;
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
		selectionGrid.CustomMinimumSize = new Vector2(selectionGrid.CustomMinimumSize.X, biggest * 125);

		for (var i = 0; i < biggest; i++)
		{
			var left = leftItems.GetArrayLength() > i ? leftItems[i] : default;
			var right = rightItems.GetArrayLength() > i ? rightItems[i] : default;

			var entry = CreateEntry(left, right);
			selectionGrid.AddChild(entry);
		}
	}

	private Control? CreateEntry(JsonElement left, JsonElement right)
	{
		var leftButton = CreateButton(left);
		var rightButton = CreateButton(right);
		
		var entry = TowerSelectionEntryScene?.Instantiate<Control>();
		if (entry == null) return null;
		
		entry.Position += new Vector2(200, 0);
		var entriesGrid = entry.GetNode<GridContainer>("entries");
		if (rightButton != null)
			entriesGrid?.AddChild(rightButton);
		if (leftButton != null)
			entriesGrid?.AddChild(leftButton);

		return entry;

	}
	
	private CenterContainer? CreateButton(JsonElement item)
	{
		if (item.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
			return null;
		
		var cell = new CenterContainer();
		cell.CustomMinimumSize = Vector2.One * 120;
		cell.Size = Vector2.One * 120;

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