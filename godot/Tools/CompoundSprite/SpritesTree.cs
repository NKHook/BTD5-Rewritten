using Godot;
using System;
using System.Collections.Generic;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;

public partial class SpritesTree : Tree
{
	public override void _Ready()
	{
		var rootItem = CreateItem();
		rootItem.SetText(0, "Root");
		Callable.From(PopulateSheets).CallDeferred();
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
			var treeItem = CreateItem(sheetItem);
			treeItem.SetText(0, infoChild.SheetName);
			PopulateSprites(treeItem, infoChild);
		}

		foreach (var spriteFrame in spriteInfo.Frames)
		{
			var treeItem = CreateItem(sheetItem);
			treeItem.SetText(0, spriteFrame.FrameName);
			PopulateFrames(treeItem, spriteFrame);
		}
	}

	private void PopulateFrames(TreeItem sheetItem, FrameInfo frameInfo)
	{
		foreach (var frameAnimation in frameInfo.Animations)
		{
			var treeItem = CreateItem(sheetItem);
			treeItem.SetText(0, frameAnimation.Name);
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
		treeItem.SetText(0, cellEntry.Name);
	}
}
