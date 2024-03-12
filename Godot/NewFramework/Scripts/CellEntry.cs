using System.Diagnostics;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public partial class CellEntry : Node
{
    private string _texturesDirPath;
    private string _filePath;
    private TextureQuality _quality;
    private Image _image = null;
    private ImageTexture _texture = null;
    
    public object Parent = null;
    public readonly string Name;
    public readonly int X;
    public readonly int Y;
    public readonly int W;
    public readonly int H;
    public readonly int Ax;
    public readonly int Ay;
    public readonly int Aw;
    public readonly int Ah;

    public CellEntry(object parent, string texturesDirPath, string filePath, TextureQuality quality, string name, int x,
        int y, int w, int h, int ax, int ay, int aw, int ah)
    {
        Parent = parent;
        _texturesDirPath = texturesDirPath;
        _filePath = filePath;
        _quality = quality;
        X = x;
        Y = y;
        W = w;
        H = h;
        Ax = ax;
        Ay = ay;
        Aw = aw;
        Ah = ah;
        
        Name = name;
    }

    public ImageTexture GetTexture()
    {
        if (_texture != null) return _texture;
        
        var image = GetImage();
        _texture = ImageTexture.CreateFromImage(image);
        return _texture;
    }
    
    public Image GetImage()
    {
        if (_image == null)
            LoadImage();

        return _image;
    }

    private void LoadImage()
    {
        var frame = Parent;
        while (frame is not FrameInfo && frame is AnimationEntry)
        {
            var entry = frame as AnimationEntry;
            frame = entry.Parent;
        }

        var properFrame = frame as FrameInfo;
        var frameImage = properFrame?.GetImage();
        Debug.Assert(frameImage is not null, "Unable to load cell because frameImage is missing for " + properFrame?.Name);

        _image = frameImage.GetRegion(new Rect2I(X, Y, W, H));
    }
}