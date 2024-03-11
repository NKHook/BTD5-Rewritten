using BloonsTD5Custom.Godot.NewFramework.Scripts;
using BloonsTD5Custom.Godot.Scripts;
using Godot;

public class CellEntry
{
    private string _texturesDirPath;
    private string _filePath;
    private TextureQuality _quality;
    private int _x;
    private int _y = 0;
    private int _w = 0;
    private int _h = 0;
    private int _ax = 0;
    private int _ay = 0;
    private int _aw = 0;
    private int _ah = 0;
    private Image _image = null;
    
    public object Parent = null;
    public readonly string Name;

    public CellEntry(object parent, string texturesDirPath, string filePath, TextureQuality quality, string name, int x,
        int y, int w, int h, int ax, int ay, int aw, int ah)
    {
        Parent = parent;
        _texturesDirPath = texturesDirPath;
        _filePath = filePath;
        _quality = quality;
        _x = x;
        _y = y;
        _w = w;
        _h = h;
        _ax = ax;
        _ay = ay;
        _aw = aw;
        _ah = ah;
        
        Name = name;
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
        if (frameImage is null) return;

        _image = frameImage.GetRegion(new Rect2I(_x, _y, _w, _h));
    }
}