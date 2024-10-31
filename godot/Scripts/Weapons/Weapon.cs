using BloonsTD5Rewritten.Screens;
using BloonsTD5Rewritten.Scripts.Bloons;
using BloonsTD5Rewritten.Scripts.Towers;
using BloonsTD5Rewritten.Scripts.Weapons.Tasks;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Weapons;

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

    /// <summary>
    /// Fires the weapon
    /// </summary>
    /// <param name="where">Where the weapon was fired</param>
    /// <param name="angle">The angle the weapon was rotated, in degrees</param>
    /// <param name="who">The bloon, if any, that is the target of the weapon</param>
    /// <param name="user">The tower that fired the weapon</param>
    public void Fire(Vector2 where, float angle, Bloon? who, BaseTower? user)
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
            task.Execute(where, angle, who, user);
        }
    }
}