using System.Collections.Generic;
using Godot;

namespace BloonsTD5Rewritten.NewFramework.Scripts.Assets;

public partial class AnimationEntry : Node
{
    private string _texturesDirPath;
    private string _filePath;
    private TextureQuality _quality;
    private readonly List<CellEntry> _cells = new();
	public List<CellEntry> Cells => _cells;

    public string AnimationName;
    public FrameInfo Parent;
    
    public AnimationEntry(FrameInfo parent, string texturesDirPath, string filePath, TextureQuality quality, string animationName)
    {
        Parent = parent;
        _texturesDirPath = texturesDirPath;
        _filePath = filePath;
        _quality = quality;
        AnimationName = animationName;
    }

    public void AddCell(CellEntry entry)
    {
        _cells.Add(entry);
    }

    public CellEntry? GetCell(string name)
    {
        return _cells.Find(entry => entry.CellName == name);
    }

    public CellEntry? FindCell(string name)
    {
        return _cells.Find(cell => cell.CellName == name);
    }
}