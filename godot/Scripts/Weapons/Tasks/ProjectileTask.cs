using System;
using System.Linq;
using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.NewFramework.Scripts.Sprites;
using BloonsTD5Rewritten.Screens;
using BloonsTD5Rewritten.Scripts.Bloons;
using BloonsTD5Rewritten.Scripts.Towers;
using Godot;
using CompoundSprite = BloonsTD5Rewritten.NewFramework.Scripts.Compound.CompoundSprite;

namespace BloonsTD5Rewritten.Scripts.Weapons.Tasks;

public partial class ProjectileTask : MoveableTask
{
    //Definition info
    public string GraphicName = "";
    public string SpriteFile = "";
    public int NumPersists;
    public bool TerminateOnZeroPersists;
    public bool RemoveOnRoundEnd;
    public bool HasLimitedDuration;
    public float LimitedDuration;
    public CollisionType CollisionType;
    public bool CollidesOnlyWithTarget;
    public StatusFlag IgnoreStatusEffect;
    public float Radius;
    public int SpinRate;
    public int[] TasksToProcessOnCollision = Array.Empty<int>();
    public int[] TasksToProcessOnTerminate = Array.Empty<int>();

    //Node info
    private int _persistsLeft = 0;
    private Node2D? _sprite;
    private Bloon? _target;
    private BaseTower? _sender;
    private Bloon? _lastCollision;
    
    public override void _Ready()
    {
        base._Ready();

        if (GraphicName != "" && SpriteFile != "")
            throw new BTD5WouldCrashException("Both a GraphicName and SpriteFile are defined in the Projectile");
        
        if (GraphicName != "")
        {
            var sprite = new Sprite();
            sprite.SpriteName = GraphicName;
            sprite.TextureName = "InGame";
            sprite.RotationDegrees = 90;
            AddChild(sprite);
            _sprite = sprite;
        }

        if (SpriteFile != "")
        {
            const string weaponSprites = "Assets/JSON/WeaponSprites/";
            var compound = new CompoundSprite();
            compound.SpriteDefinitionRes = weaponSprites + SpriteFile;
            AddChild(compound);
            _sprite = compound;
        }

        _persistsLeft = NumPersists;
    }

    public override void Execute(Vector2 where, float angle, Bloon? who, BaseTower? user)
    {
        var gameScreen = ScreenManager.Instance().CurrentScreen as GameScreen;
        var taskObjects = gameScreen?.GetNode<TaskObjectManager>("TaskObjects");
        
        if (Clone() is not ProjectileTask clone) return;

        clone._target = who;
        clone._sender = user;
        clone.Position = where;
        clone.Rotation = Mathf.DegToRad(angle);
        clone.Angle = angle;
        if (clone is MoveableTask { Movement: not null } movable)
        {
            movable.Movement.Direction = Vector2.FromAngle(Mathf.DegToRad(angle));
            movable.Origin = where;
            movable.Sender = user;
            movable.Target = who;
        }
            
        taskObjects?.AddObject(clone);
    }

    public override void _Process(double delta)
    {
        if (CollidesOnlyWithTarget && _target != null)
        {
            foreach (var taskId in TasksToProcessOnCollision)
            {
                if (DisabledTasks.Contains(taskId)) continue;

                var task = Tasks[taskId];
                task.Execute(_target.Position, 0.0f, _target, _sender);
            }
            base._Process(delta);
            
            return;
        }
        
        base._Process(delta);

        if (_sprite != null)
            _sprite.RotationDegrees += SpinRate * (float)delta;

        var bloons = BloonManager.Instance?.Objects;
        if (bloons == null) return;
        
        foreach (var bloon in bloons)
        {
            if (!bloon.Collided(this)) continue;
            
            switch (CollisionType)
            {
                case CollisionType.Once:
                    if (_lastCollision != bloon)
                    {
                        if (_persistsLeft > 0)
                            Collided(bloon);
                        
                        _persistsLeft--;
                        _lastCollision = bloon;
                    }
                    break;
                case CollisionType.Continual:
                    if (_persistsLeft > 0)
                        Collided(bloon);
                    _persistsLeft--;
                    break;
                case CollisionType.None:
                    Collided(bloon);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        if (TerminateOnZeroPersists && _persistsLeft <= 0)
            Terminate();
    }

    private void Collided(Bloon? with)
    {
        var taskIds = TasksToProcessOnCollision.Length > 0
            ? TasksToProcessOnCollision
            : Tasks.Select((task, i) => i).ToArray();
        foreach (var taskId in taskIds)
        {
            if (DisabledTasks.Contains(taskId)) continue;
                
            var task = Tasks[taskId];
            task.Execute(with?.Position ?? Vector2.Zero, 0.0f, with, null);
        }
    }

    public override void Terminate()
    {
        base.Terminate();

        foreach (var taskId in TasksToProcessOnTerminate)
        {
            if (DisabledTasks.Contains(taskId)) continue;

            var task = Tasks[taskId];
            task.Execute(Position, RotationDegrees, null, null);
        }
    }

    public override object Clone()
    {
        var clone = base.Clone() as ProjectileTask;
        clone!.GraphicName = GraphicName;
        clone.SpriteFile = SpriteFile;
        clone.NumPersists = NumPersists;
        clone.TerminateOnZeroPersists = TerminateOnZeroPersists;
        clone.CollisionType = CollisionType;
        clone.CollidesOnlyWithTarget = CollidesOnlyWithTarget;
        clone.SpinRate = SpinRate;
        clone.DisabledTasks = DisabledTasks;
        clone.TasksToProcessOnCollision = TasksToProcessOnCollision;
        clone.TasksToProcessOnTerminate = TasksToProcessOnTerminate;
        return clone;
    }
}