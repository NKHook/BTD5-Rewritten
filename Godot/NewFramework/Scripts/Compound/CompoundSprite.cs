using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;

public partial class CompoundSprite : Node2D
{
    [ExportCategory("Compound Sprite")]
    [Export] public string SpriteDefinitionRes = "";
    [Export] public bool Animating = true;

    private TimelineInterpolator? _timeline;
    private List<CellEntry> _usedCells = new();
    private readonly SparseList<ActorState> _initialStates = new();
    private readonly SparseList<CellEntry> _childCells = new();

    public readonly EventHandler? Loaded = null;
    public bool FullyLoaded { get; private set; }

    public float Time
    {
        get => _timeline?.Time ?? 0.0f;
        set
        {
            if (_timeline != null) _timeline.Time = value;
        }
    }

    public bool Loop
    {
        get => _timeline?.Loop ?? true;
        set
        {
            if (_timeline != null) _timeline.Loop = value;
        }
    }

    public float Duration => _timeline?.Length ?? 0.0f;

    public CompoundSprite LoadCompoundSprite(string sprite)
    {
        var compoundSprite = new CompoundSprite();
        compoundSprite.SpriteDefinitionRes = (Path.GetDirectoryName(SpriteDefinitionRes) + "/" + sprite).Replace("\\", "/");
        compoundSprite.Animating = Animating;
        return compoundSprite;
    }

    private static void SetSpriteCell(Sprite2D sprite, CellEntry? cell)
    {
        sprite.Texture = cell?.GetTexture();
        sprite.RegionEnabled = true;
        sprite.RegionRect = cell?.GetRegion() ?? new Rect2();
    }

    private Sprite2D LoadSingleSprite(CellEntry cell, ActorState state)
    {
        var spriteObj = new Sprite2D();
        SetSpriteCell(spriteObj, cell);
        state.ApplyAndAlign(spriteObj);
        return spriteObj;
    }

    private Node2D? LoadActor(JsonWrapper actor)
    {
        string sprite = actor["sprite"];
        Debug.Assert(sprite != string.Empty);
        var type = actor["type"].EnumValue<ActorTypes>();
        var uid = actor["uid"].GetInt32();

        Node2D result;
        switch (type)
        {
            case ActorTypes.Sprite:
                var cell = _usedCells.FirstOrDefault(used => used.CellName == sprite);
                if (cell == null)
                    return null;

                var state = new ActorState(cell, actor);
                _initialStates[uid] = state;
                _childCells[uid] = cell;

                result = LoadSingleSprite(cell, state);
                break;
            case ActorTypes.CompoundSprite:
                result = LoadCompoundSprite(sprite!);
                _initialStates[uid] = new ActorState(null, actor);
                _childCells[uid] = null;
                break;
            case ActorTypes.Invalid:
            default:
                throw new ArgumentOutOfRangeException();
        }

        result.Name = uid.ToString();
        return result;
    }

    public static List<CellEntry> LoadSpriteInfo(JsonWrapper stageOptions)
    {
        var infosJson = stageOptions["SpriteInfo"];

        return (from info in infosJson.EnumerateArray()
            let sprite = info["SpriteInfo"]
            let texture = info["Texture"]
            select TextureLoader.Instance().FindCell(sprite, texture).As<CellEntry>()
            into cell
            where cell != null
            select cell).ToList();
    }

    public static TimelineInterpolator LoadStageOptions(JsonWrapper stageOptions)
    {
        var duration = stageOptions["StageLength"];
        return new TimelineInterpolator(duration);
    }

    public void Initialize()
    {
        _timeline = null;
        _usedCells.Clear();
        _initialStates.Clear();
        _childCells.Clear();
        foreach (var child in GetChildren())
        {
            child.Free();
        }

        if (SpriteDefinitionRes == "")
            return;
        var spriteDefinitionJson = JetFileImporter.Instance().GetJsonParsed(SpriteDefinitionRes);

        var stageOptions = spriteDefinitionJson["stageOptions"];
        if (stageOptions != null && stageOptions.ValueKind != JsonType.Null)
        {
            _usedCells = LoadSpriteInfo(stageOptions);
            _timeline = LoadStageOptions(stageOptions);
        }

        var actors = spriteDefinitionJson["actors"] ?? throw new BTD5WouldCrashException();
        var sprites = actors.EnumerateArray().Select(LoadActor).ToArray();
        foreach (var spriteObj in sprites)
        {
            if (spriteObj != null)
                AddChild(spriteObj);
        }

        if (!spriteDefinitionJson.TryGetProperty("timelines", out var timelinesJson)) return;
        
        foreach (var timelineJson in timelinesJson.EnumerateArray())
        {
            var uid = timelineJson["spriteuid"]?.GetInt32() ?? throw new BTD5WouldCrashException();
            var stagesJson = timelineJson["stage"];
            if(stagesJson == null || stagesJson.ValueKind == JsonType.Null)
                continue;

            var stages = new List<ActorState>();
            foreach (var stageJson in stagesJson.EnumerateArray())
            {
                if(stageJson.ValueKind == JsonType.Null)
                    continue;
            
                //Prevent states with the same time overwriting eachother
                //the game only uses the first one at the same time for some reason
                var time = stageJson["Time"]?.GetFloat() ?? throw new BTD5WouldCrashException();
                if (stages.Where(stage => stage.Time.Equals(time)).ToArray().Length > 0)
                    continue;

                var cell = _childCells[uid];
                stages.Add(new ActorState(cell, stageJson));
            }

            Node2D? node = null;
            foreach (var child in GetChildren())
            {
                if (int.Parse(child.Name) != uid) continue;
            
                node = child as Node2D;
                if (node != null)
                    break;
            }
            Debug.Assert(node != null);
        
            _timeline?.AddTimeline(uid, node!, stages);
            _timeline?.SetInitialState(uid, _initialStates[uid]);
        }
    }

    private void RefreshTextures()
    {
        foreach (var child in GetChildren())
        {
            if (!int.TryParse(child?.Name.ToString(), out var uid))
                continue;

            switch (child)
            {
                case Sprite2D sprite:
                    SetSpriteCell(sprite, _childCells[uid]);
                    break;
                case CompoundSprite compound:
                    compound.RefreshTextures();
                    break;
            }
        }
    }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() => Initialize();

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!FullyLoaded)
        {
            FullyLoaded = _usedCells.All(cell => cell.GetTexture() != null);
            if (FullyLoaded)
            {
                RefreshTextures();
                Loaded?.Invoke(this, null!);
            }
        }
        
        if (Animating)
            _timeline?.Tick((float)delta);
        
        foreach (var childNode in GetChildren())
        {
            var child = childNode as Node2D;
            
            if (!int.TryParse(child?.Name.ToString(), out var uid))
                continue;

            var state = _timeline?.GetStateForUid(uid) ?? _initialStates[uid];
            Debug.Assert(state is not null);
            
            if (state?.Color != Colors.White && state?.Color.A != 0)
                state?.ApplyColor(child);
            state?.Apply(child);
        }
    }
}
