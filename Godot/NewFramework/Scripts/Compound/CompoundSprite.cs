#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

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

    public Node2D LoadCompoundSprite(string sprite)
    {
        var compoundSprite = new CompoundSprite();
        compoundSprite.SpriteDefinitionRes = (Path.GetDirectoryName(SpriteDefinitionRes) + "/" + sprite).Replace("\\", "/");
        compoundSprite.Animating = Animating;
        return compoundSprite;
    }

    public Sprite2D LoadSingleSprite(CellEntry cell, ActorState state)
    {
        var spriteObj = new Sprite2D();
        spriteObj.Texture = cell.GetTexture();
        spriteObj.RegionEnabled = true;
        spriteObj.RegionRect = cell.GetRegion();
        state.ApplyAndAlign(spriteObj);
        return spriteObj;
    }

    public Node2D LoadActor(JsonElement actor)
    {
        var sprite = actor.GetProperty("sprite").GetString();
        Debug.Assert(sprite != null);
        var type = (ActorTypes)actor.GetProperty("type").GetInt32();
        var uid = actor.GetProperty("uid").GetInt32();

        Node2D result;
        switch (type)
        {
            case ActorTypes.Sprite:
                var cell = _usedCells.FirstOrDefault(used => used.Name == sprite);
                Debug.Assert(cell != null);

                var state = new ActorState(cell, actor);
                _initialStates[uid] = state;
                _childCells[uid] = cell;

                result = LoadSingleSprite(cell!, state);
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

    public static List<CellEntry> LoadSpriteInfo(JsonElement stageOptions)
    {
        var infosJson = stageOptions.GetProperty("SpriteInfo");

        return (from info in infosJson.EnumerateArray()
            let sprite = info.GetProperty("SpriteInfo").GetString()
            let texture = info.GetProperty("Texture").GetString()
            select TextureLoader.Instance().FindCell(sprite, texture).As<CellEntry>()
            into cell
            where cell != null
            select cell).ToList();
    }

    public static TimelineInterpolator? LoadStageOptions(JsonElement stageOptions)
    {
        var duration = stageOptions.GetProperty("StageLength").GetSingle();
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

        Debug.Assert(SpriteDefinitionRes != "");
        var spriteDefinitionJson = JetFileImporter.Instance().GetJsonParsed(SpriteDefinitionRes);

        var stageOptions = spriteDefinitionJson.GetProperty("stageOptions");
        _usedCells = LoadSpriteInfo(stageOptions);
        _timeline = LoadStageOptions(stageOptions);

        var actors = spriteDefinitionJson.GetProperty("actors");
        foreach (var spriteObj in actors.EnumerateArray().Select(LoadActor))
        {
            AddChild(spriteObj);
        }

        var timelinesJson = spriteDefinitionJson.GetProperty("timelines");
        foreach (var timelineJson in timelinesJson.EnumerateArray())
        {
            var uid = timelineJson.GetProperty("spriteuid").GetInt32();
            var stagesJson = timelineJson.GetProperty("stage");
            if(stagesJson.ValueKind == JsonValueKind.Null)
                continue;

            var stages = new List<ActorState>();
            foreach (var stageJson in stagesJson.EnumerateArray())
            {
                if(stageJson.ValueKind == JsonValueKind.Null)
                    continue;
                
                //Prevent states with the same time overwriting eachother
                //the game only uses the first one at the same time for some reason
                var time = stageJson.GetProperty("Time").GetSingle();
                if (stages.Where(stage => stage.Time.Equals(time)).ToArray().Length > 0)
                    continue;

                var cell = _childCells[uid];
                stages.Add(new ActorState(cell, stageJson));
            }

            Node2D? node = null;
            foreach (var child in GetChildren(false))
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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() => Initialize();

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!Animating)
            return;
        
        _timeline?.Tick((float)delta);
        foreach (var childNode in GetChildren(false))
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