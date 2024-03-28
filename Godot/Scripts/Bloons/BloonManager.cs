using BloonsTD5Rewritten.Godot.NewFramework.Scripts;

namespace BloonsTD5Rewritten.Godot.Scripts.Bloons;

public partial class BloonManager : ObjectManager<Bloon>
{
    private float _time;

    public override void _Process(double delta)
    {
        base._Process(delta);

        _time += (float)delta;
        if (!(_time >= 1.0f)) return;
        
        _time -= _time;
        var bloon = BloonFactory.Instance.Instantiate("Red");
        AddObject(bloon!);
    }
}