using Godot;

namespace BloonsTD5Rewritten.Scripts.Bloons;

public partial class BloonManager : BloonsTD5Rewritten.NewFramework.Scripts.ObjectManager<Bloon>
{
    public static BloonManager? Instance;
    private float _time;
    //private Path2D[]? _bloonPaths;

    private Label? _countDebugLabel;
    
    public override void _Ready()
    {
        Instance = this;
        base._Ready();
        
        _countDebugLabel = GetNode<Label>("/root/game_root/debug_overlay/overlay/bloon_count_display");
        
        /*var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var mapPath = gameScreen?.GetNode<MapPath>("map_path");
        _bloonPaths = mapPath?.GetChildren().OfType<Path2D>().ToArray();*/
    }

    public override void _ExitTree()
    {
        Instance = null;
        base._ExitTree();
    }

    public override void RemoveObject(Bloon obj)
    {
        obj.PathFollower?.QueueFree();
        obj.QueueFree();
        base.RemoveObject(obj);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_countDebugLabel != null) _countDebugLabel.Text = Count + " bloons";

        //var progressThisFrame = 3.0f * _speed * _speedMultiplier * (float)delta;
        /*var bloons = Objects.ToArray();
        foreach (var bloon in bloons)
        {
            bloon.PathFollower!.Progress += 3.0f * bloon.Speed * (float)delta;
        }

        foreach (var bloon in bloons)
        {
            if (bloon.PathFollower?.ProgressRatio >= 1.0f)
                RemoveObject(bloon);
        }*/
        
        _time += (float)delta;
        if (!(_time >= 0.1f)) return;
        
        _time -= _time;
        var newbloon = BloonFactory.Instance.Instantiate(BloonType.Red);
        AddObject(newbloon!);
    }
}