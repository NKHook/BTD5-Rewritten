using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BloonsTD5Custom.Godot.Scripts;
using Godot;

namespace BloonsTD5Custom.Godot.NewFramework.Scripts;

public class FrameInfo
{
    private SpriteInfo _parent;
    private string _texturesDirPath;
    private string _filePath;
    private TextureQuality _quality;
    private int _texw;
    private int _texh;
    private TextureType _type;
    private readonly List<AnimationEntry> _animations = new();
    private readonly List<CellEntry> _cells = new();
    private Image _frameImage;
    
    public string Name;
    
    public FrameInfo(SpriteInfo parent, string texturesDirPath, string filePath, TextureQuality quality, string name, int texw, int texh, TextureType type)
    {
        _parent = parent;
        _texturesDirPath = texturesDirPath;
        _filePath = filePath;
        _quality = quality;
        Name = name;
        _texw = texw;
        _texh = texh;
        _type = type;
    }

    public Image GetImage()
    {
        if (_frameImage == null)
            LoadFrame();
        
        return _frameImage;
    }
    
    public void LoadFrame()
    {
        var extension = _type switch
        {
            TextureType.JPNG => ".jpng",
            TextureType.JPG => ".jpg",
            TextureType.PNG => ".png",
            TextureType.INVALID => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
        while (true)
        {
            var dir = Path.GetDirectoryName(_filePath);
            var file = dir + "/" + Name + extension;
            //GD.Print("Loading frame: " + file);

            switch (_type)
            {
                case TextureType.PNG:
                case TextureType.JPG:
                    _frameImage = global::Godot.Image.LoadFromFile(file);
                    break;
                case TextureType.JPNG:
                    _frameImage = JpngLoader.LoadJpngTexture(file, _texw, _texh);
                    if (_frameImage == null)
                    {
                        _type = TextureType.PNG;
                        continue;
                    }

                    break;
                case TextureType.INVALID:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            break;
        }
    }

    public void AddAnimation(AnimationEntry entry)
    {
        _animations.Add(entry);
    }

    public void AddCell(CellEntry entry)
    {
        _cells.Add(entry);
    }

    public AnimationEntry GetAnimation(string name)
    {
        return _animations.Find(entry => entry.Name == name);
    }

    public CellEntry GetCell(string name)
    {
        return _cells.Find(entry => entry.Name == name);
    }

    public CellEntry FindCell(string name)
    {
        foreach (var result in _animations.Select(animation => animation.FindCell(name)).Where(result => result != null))
        {
            return result;
        }

        return _cells.Find(cell => cell.Name == name);
    }
}