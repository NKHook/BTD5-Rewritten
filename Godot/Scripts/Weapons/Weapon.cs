using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public partial class Weapon : Node2D
{
    private readonly WeaponInfo _definition;
    private readonly WeaponTask[]? _tasks;
    private TaskObjectManager? _taskObjects;
    public Weapon(WeaponInfo definition)
    {
        _definition = definition;
        _tasks = _definition.Tasks.Clone() as WeaponTask[];
    }

    public float Range => _definition.TargetRange;

    public bool Cooled;
    public float CooldownTimer;
    public bool FireReady;
    public float FireTimer;

    public override void _Process(double delta)
    {
        base._Process(delta);
        CooldownTimer += (float)delta;
        Cooled = FireTimer >= _definition.CooldownTime;

        if (!Cooled) return;
        
        FireTimer += (float)delta;
        FireReady = FireTimer >= _definition.FireDelayTime;
    }

    public void Fire(Vector2 where, Vector2 direction)
    {
        CooldownTimer = 0.0f;
        FireTimer = 0.0f;
        if (_taskObjects == null)
        {
            var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
            _taskObjects = gameScreen?.GetNode<TaskObjectManager>("TaskObjects");
            return;
        }
        
        if (_tasks == null || _tasks.Length == 0) return;

        foreach (var task in _tasks)
        {
            if (task.Duplicate() is not WeaponTask clone) continue;
            
            clone.Position = where;
            if (clone is MoveableTask { Movement: not null } movable) 
                movable.Movement.Direction = direction;

            _taskObjects.AddObject(clone);
        }
    }
}