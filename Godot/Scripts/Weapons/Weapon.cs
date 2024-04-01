using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons;

public partial class Weapon
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

    public bool Cooled => CooldownTimer >= _definition.CooldownTime;
    public float CooldownTimer;
    public bool FireReady => FireTimer >= _definition.FireDelayTime;
    public float FireTimer;

    public void UpdateCooldown(float delta)
    {
        CooldownTimer += delta;
    }

    public void UpdateFire(float delta)
    {
        FireTimer += delta;
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
            task.Execute(where, direction, null);
        }
    }
}