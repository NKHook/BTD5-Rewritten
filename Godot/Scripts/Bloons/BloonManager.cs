using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

public partial class BloonManager : ObjectManager<Bloon>
{
    public static BloonManager? Instance;
    private float _time;

    private Label? _countDebugLabel;
    
    public override void _Ready()
    {
        Instance = this;
        base._Ready();
        
        _countDebugLabel = GetNode<Label>("/root/game_root/debug_overlay/overlay/bloon_count_display");
    }

    public override void _ExitTree()
    {
        Instance = null;
        base._ExitTree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_countDebugLabel != null) _countDebugLabel.Text = Count + " bloons";

        _time += (float)delta;
        if (!(_time >= 0.1f)) return;
        
        _time -= _time;
        var bloon = BloonFactory.Instance.Instantiate(BloonType.Red);
        AddObject(bloon!);
    }
}