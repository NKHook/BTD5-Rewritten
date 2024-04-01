using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

public partial class BloonManager : ObjectManager<Bloon>
{
    public static BloonManager? Instance;
    private float _time;

    public override void _Ready()
    {
        Instance = this;
        base._Ready();
    }

    public override void _ExitTree()
    {
        Instance = null;
        base._ExitTree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        _time += (float)delta;
        if (!(_time >= 0.5f)) return;
        
        _time -= _time;
        var bloon = BloonFactory.Instance.Instantiate(BloonType.Red);
        AddObject(bloon!);
    }
}