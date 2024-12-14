using Godot;
using System;

public partial class NewTab : Button
{
	[Export] public TabContainer EditorTabs;
	[Export] public PackedScene EditScene;
	
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		
	}
	
	private void OpenFromFile()
	{
		var assetImporterConfig = GetNode<Node>("/root/AssetImporterConfig");
		
		var dialog = new FileDialog();
		dialog.Title = "Open File";
		dialog.Access = FileDialog.AccessEnum.Filesystem;
		dialog.CurrentDir = assetImporterConfig.Get("game_dir").AsString();
		dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
		dialog.Filters = new []
		{
			"*.*;*.*;All files",
			"*.json;*.json;JSON files"
		};
		dialog.UseNativeDialog = true;
		//dialog.FileSelected += OpenSpriteFile;
		dialog.Show();
	}
}
