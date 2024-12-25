using Godot;
using System;
using System.Collections.Generic;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;

public partial class SpritesTree : Tree
{
	[Export] public Sprite2D? Preview;
	[Export] public LineEdit? SearchBar;
	
	public override void _Ready()
	{
		var rootItem = CreateItem();
		rootItem.SetText(0, "Root");
		Callable.From(PopulateSheets).CallDeferred();
		
		ItemSelected += OnItemSelected;
		SearchBar!.TextChanged += SearchBarOnTextChanged;
	}

	private void SearchBarOnTextChanged(string newtext)
	{
		var items = new List<TreeItem>();
		CollectItemsHelper(ref items, GetRoot());
		
		foreach (var treeItem in items)
		{
			if (!treeItem.GetText(0).Contains(SearchBar?.Text ?? string.Empty)) continue;
			
			ScrollToItem(treeItem, true);
			return;
		}
	}

	private void CollectItemsHelper(ref List<TreeItem> items, TreeItem current)
	{
		foreach (var treeItem in current.GetChildren())
		{
			items.Add(treeItem);
			CollectItemsHelper(ref items, treeItem);
		}
	}

	private void OnItemSelected()
	{
		var selected = GetSelected();
		if (selected == null)
		{
			GD.Print("Nothing selected");
			return;
		}

		var textures = TextureLoader.Instance();
		// Find the frame
		FrameInfo? frameInfo = null;
		var current = selected;
		while (current != null && frameInfo == null)
		{
			frameInfo = textures?.FindFrame(current.GetText(0));
			current = current.GetParent();
		}

		if (frameInfo == null)
		{
			GD.Print("No frame");
			return;
		}

		var cell = frameInfo.FindCell(selected.GetText(0));
		if (cell == null)
		{
			GD.Print("No cell");
			Preview!.Texture = frameInfo.GetTexture();
			Preview.RegionEnabled = false;
			return;
		}

		Preview!.Texture = cell.GetTexture();
		Preview.RegionEnabled = true;
		Preview.RegionRect = cell.GetRegion();
	}

	private void PopulateSheets()
	{
		foreach (var spriteInfo in TextureLoader.Instance()?.SpritesRoot ?? new List<SpriteInfo>())
		{
			var treeItem = CreateItem();
			treeItem.SetText(0, spriteInfo.SheetName);
			PopulateSprites(treeItem, spriteInfo);
		}
	}

	private void PopulateSprites(TreeItem sheetItem, SpriteInfo spriteInfo)
	{
		foreach (var infoChild in spriteInfo.Children)
		{
			//if (SearchBar?.Text != string.Empty && !infoChild.SheetName.Contains(SearchBar!.Text)) continue;
			
			var treeItem = CreateItem(sheetItem);
			treeItem.SetText(0, infoChild.SheetName);
			PopulateSprites(treeItem, infoChild);
		}

		foreach (var spriteFrame in spriteInfo.Frames)
		{
			//if (SearchBar?.Text != string.Empty && !spriteFrame.FrameName.Contains(SearchBar!.Text)) continue;
			
			var treeItem = CreateItem(sheetItem);
			treeItem.SetText(0, spriteFrame.FrameName);
			PopulateFrames(treeItem, spriteFrame);
		}
	}

	private void PopulateFrames(TreeItem sheetItem, FrameInfo frameInfo)
	{
		foreach (var frameAnimation in frameInfo.Animations)
		{
			//if (SearchBar?.Text != string.Empty && !frameAnimation.AnimationName.Contains(SearchBar!.Text)) continue;
			
			var treeItem = CreateItem(sheetItem);
			treeItem.SetText(0, frameAnimation.AnimationName);
			PopulateAnimation(treeItem, frameAnimation);
		}

		foreach (var frameCell in frameInfo.Cells)
		{
			PopulateCell(sheetItem, frameCell);
		}
	}

	private void PopulateAnimation(TreeItem sheetItem, AnimationEntry animation)
	{
		foreach (var animationCell in animation.Cells)
		{
			PopulateCell(sheetItem, animationCell);
		}
	}

	private void PopulateCell(TreeItem cellParent, CellEntry cellEntry)
	{
		var treeItem = CreateItem(cellParent);
		treeItem.SetText(0, cellEntry.CellName);
	}
}
