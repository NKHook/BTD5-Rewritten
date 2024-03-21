using System.Diagnostics;
using System.Threading.Tasks;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;

public partial class CellEntry : Node
{
    private string _texturesDirPath;
    private string _filePath;
    private TextureQuality _quality;
    
    public object Parent;
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

    public Rect2 GetRegion() => new(X, Y, W, H);
    
    public ImageTexture? GetTexture()
    {
        var frame = GetFrame();
        return frame?.GetTexture(); 
    }

    private FrameInfo? GetFrame()
    {
        var frame = Parent;
        while (frame is not FrameInfo && frame is AnimationEntry entry)
        {
            frame = entry?.Parent;
        }

        var properFrame = frame as FrameInfo;
        return properFrame;
    }
}