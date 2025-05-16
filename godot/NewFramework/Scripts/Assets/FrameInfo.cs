using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using FileAccess = Godot.FileAccess;

namespace BloonsTD5Rewritten.NewFramework.Scripts.Assets;

public partial class FrameInfo : Node
{
    private SpriteInfo _parent;
    private string _texturesDirPath;
    private string _filePath;
    private TextureQuality _quality;
    private int _texw;
    private int _texh;
    private TextureType _type;
    private readonly List<AnimationEntry> _animations = new();
	public List<AnimationEntry> Animations => _animations;
    private readonly List<CellEntry> _cells = new();
	public List<CellEntry> Cells => _cells;
    private Image? _frameImage;
    private ImageTexture? _frameTexture;
    private Task<Image>? _imageTask;
    
    public string FrameName;
    
    public FrameInfo(SpriteInfo parent, string texturesDirPath, string filePath, TextureQuality quality, string frameName, int texw, int texh, TextureType type)
    {
        _parent = parent;
        _texturesDirPath = texturesDirPath;
        _filePath = filePath;
        _quality = quality;
        FrameName = frameName;
        _texw = texw;
        _texh = texh;
        _type = type;
    }

    public float GetQualityScale()
    {
        return _quality switch
        {
            TextureQuality.Low => 4.0f,
            TextureQuality.Mobile => 3.0f,
            TextureQuality.Tablet => 2.0f,
            TextureQuality.High => 1.5f,
            TextureQuality.Ultra => 1.0f,
            TextureQuality.Invalid => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public ImageTexture? GetTexture()
    {
        if (_frameTexture != null)
            return _frameTexture;
        
        _imageTask ??= GetImage();
        if (_imageTask.IsCompleted)
        {
            _frameTexture = ImageTexture.CreateFromImage(_imageTask.Result);
        }
        return _frameTexture;
    }
    
    public async Task<Image> GetImage()
    {
        return _frameImage ??= await LoadFrame();
    }

    private string GetFileFaultTolerant()
    {
        while (_type != TextureType.INVALID)
        {
            var extension = _type switch
            {
                TextureType.JPNG => ".jpng",
                TextureType.JPG => ".jpg",
                TextureType.PNG => ".png",
                TextureType.INVALID => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };
            var dir = Path.GetDirectoryName(_filePath);
            var file = dir + "/" + FrameName + extension;
            if (FileAccess.FileExists(file))
            {
                return file;
            }

            _type--;
        }
        throw new FileNotFoundException($"No texture is available for frame {FrameName} (No known types are found)");
    }

    private async Task<Image> LoadFrame()
    {
        Image? frameImage;
        while (true)
        {
            var file = GetFileFaultTolerant();
            GD.Print("Loading frame: " + file);
            switch (_type)
            {
                case TextureType.PNG:
                case TextureType.JPG:
                    frameImage = Image.LoadFromFile(file);
                    break;
                case TextureType.JPNG:
                    frameImage = await JpngLoader.LoadJpngTexture(file, _texw, _texh);
                    if (frameImage == null)
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
        return frameImage;
    }

    public void AddAnimation(AnimationEntry entry)
    {
        _animations.Add(entry);
    }

    public void AddCell(BloonsTD5Rewritten.NewFramework.Scripts.Assets.CellEntry entry)
    {
        _cells.Add(entry);
    }

    public AnimationEntry? GetAnimation(string name)
    {
        return _animations.Find(entry => entry.AnimationName == name);
    }

    public BloonsTD5Rewritten.NewFramework.Scripts.Assets.CellEntry? GetCell(string name)
    {
        return _cells.Find(entry => entry.CellName == name);
    }

    public BloonsTD5Rewritten.NewFramework.Scripts.Assets.CellEntry? FindCell(string name)
    {
        foreach (var result in _animations.Select(animation => animation.FindCell(name)).Where(result => result != null))
        {
            return result;
        }

        return _cells.Find(cell => cell.CellName == name);
    }
}