using System;
using System.Linq;
using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Screens.Components;
using BloonsTD5Rewritten.Scripts;
using BloonsTD5Rewritten.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Screens;

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

		var rightItems = rightOrder["Items"];
		var leftItems = leftOrder["Items"];

		var selectionGrid = GetNode<GridContainer>("tower_selection_scroll/tower_selection_grid");
		var biggest = Math.Max(leftItems!.EnumerateArray().Count(), rightItems!.EnumerateArray().Count());
		selectionGrid.CustomMinimumSize = new Vector2(selectionGrid.CustomMinimumSize.X, biggest * 125);

		for (var i = 0; i < biggest; i++)
		{
			var left = leftItems.ArrayLen() > i ? leftItems[i] : default;
			var right = rightItems.ArrayLen() > i ? rightItems[i] : default;

			var entry = CreateEntry(left!, right!);
			selectionGrid.AddChild(entry);
		}
	}
	
	private Control? CreateEntry(JsonWrapper left, JsonWrapper right)
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
	
	private CenterContainer? CreateButton(JsonWrapper? item)
	{
		if (item == null || item.ValueKind is JsonType.Null)
			return null;
		
		var cell = new CenterContainer();
		cell.CustomMinimumSize = Vector2.One * 120;
		cell.Size = Vector2.One * 120;

		string icon = item["Icon"]!;
		string factoryName = item["FactoryName"]!;
		var button = TowerSelectionButtonScene?.Instantiate<TowerSelectionButton>();
		button!.Name = factoryName + "_button";
		button.TowerIcon = icon!;
		button.FactoryName = factoryName!;
		button.CustomMinimumSize = Vector2.One * 120;

		button.Pressed += () =>
		{
			var floatingTower = TowerFactory.Instance.Instantiate(button.FactoryName);
			var currentScreen = ScreenManager.Instance().CurrentScreen;
			var towerManager = currentScreen?.GetNode<TowerManager>("TowerManager");
			towerManager?.SetFloating(floatingTower);
		};

		cell.AddChild(button);
		return cell;
	}
}