using System;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using BloonsTD5Rewritten.Godot.Screens;
using BloonsTD5Rewritten.Godot.Scripts.Bloons;
using BloonsTD5Rewritten.Godot.Scripts.Towers;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Weapons.Tasks;

public partial class ProjectileTask : MoveableTask
{
    //Definition info
    public string GraphicName = "";
    public string SpriteFile = "";
    public int NumPersists;
    public bool TerminateOnZeroPersists;
    public CollisionType CollisionType;
    public int SpinRate;
    public int[] DisabledTasks = Array.Empty<int>();
    public int[] TasksToProcessOnCollision = Array.Empty<int>();
    public int[] TasksToProcessOnTerminate = Array.Empty<int>();

    //Node info
    private int _persistsLeft = 0;
    private Node2D? _sprite;
    
    public override void _Ready()
    {
        base._Ready();

        if (GraphicName != "" && SpriteFile != "")
            throw new BTD5WouldCrashException("Both a GraphicName and SpriteFile are defined in the Projectile");

        if (GraphicName == "" && SpriteFile == "")
            throw new BTD5WouldCrashException("Neither a GraphicName or SpriteFile are defined in the Projectile");
        
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
        
        clone.Position = where;
        clone.Rotation = Mathf.DegToRad(angle);
        clone.Angle = angle;
        if (clone is MoveableTask { Movement: not null } movable)
        {
            movable.Movement.Direction = Vector2.FromAngle(Mathf.DegToRad(angle));
            movable.Origin = where;
        }
            
        taskObjects?.AddObject(clone);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_sprite != null)
            _sprite.RotationDegrees += SpinRate * (float)delta;

        var bloons = BloonManager.Instance?.Objects;
        if (bloons == null) return;
        
        foreach (var bloon in bloons)
        {
            if (!bloon.Collided(this)) continue;

            foreach (var taskId in TasksToProcessOnCollision)
            {
                if (DisabledTasks.Contains(taskId)) continue;
                    
                var task = Tasks[taskId];
                task.Execute(bloon.Position, 0.0f, bloon, null);
            }

            switch (CollisionType)
            {
                case CollisionType.Once:
                    _persistsLeft--;
                    if (_persistsLeft <= 0)
                        Terminate();
                    break;
                case CollisionType.Continual:
                case CollisionType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        clone.SpinRate = SpinRate;
        clone.DisabledTasks = DisabledTasks;
        clone.TasksToProcessOnCollision = TasksToProcessOnCollision;
        clone.TasksToProcessOnTerminate = TasksToProcessOnTerminate;
        return clone;
    }
}