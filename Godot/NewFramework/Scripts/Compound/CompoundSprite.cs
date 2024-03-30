using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;

public partial class CompoundSprite : Node2D
{
    [ExportCategory("Compound Sprite")]
    [Export] public string SpriteDefinitionRes = "";

    private AnimationPlayer? _animationPlayer;
    private Animation? _animation;
    private bool _animating = true;
    private List<CellEntry> _usedCells = new();
    private readonly SparseList<ActorState> _initialStates = new();
    private readonly SparseList<CellEntry> _childCells = new();

    private static int _animId = 0;
    
    public readonly EventHandler? Loaded = null;
    public bool FullyLoaded { get; private set; }

    public float Time
    {
        get => (float)(_animationPlayer?.CurrentAnimationPosition ?? 0.0);
        set => _animationPlayer?.Seek(value);
    }

    public bool Loop
    {
        get => _animation?.LoopMode is Animation.LoopModeEnum.Linear or Animation.LoopModeEnum.Pingpong;
        set
        {
            if (_animation != null)
                _animation.LoopMode = value ? Animation.LoopModeEnum.Linear : Animation.LoopModeEnum.None;
        }
    }

    public float Duration => _animation?.Length ?? 0.0f;

    public CompoundSprite LoadCompoundSprite(string sprite)
    {
        var compoundSprite = new CompoundSprite();
        compoundSprite.SpriteDefinitionRes = (Path.GetDirectoryName(SpriteDefinitionRes) + "/" + sprite).Replace("\\", "/");
        if (Playing)
        {
            compoundSprite.PlayAnimation();
        }
        else
        {
            compoundSprite.PauseAnimation();
        }
        return compoundSprite;
    }

    private static void SetSpriteCell(Sprite2D sprite, CellEntry? cell)
    {
        sprite.Texture = cell?.GetTexture();
        sprite.RegionEnabled = true;
        sprite.RegionRect = cell?.GetRegion() ?? new Rect2();
    }

    private Sprite LoadSingleSprite(CellEntry cell, ActorState state)
    {
        var spriteObj = new Sprite();
        spriteObj.Cell = cell;
        state.ApplyAndAlign(spriteObj);
        state.ApplyColor(spriteObj);
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

    public static Animation LoadStageOptions(JsonWrapper stageOptions)
    {
        var result = new Animation();
        var duration = stageOptions["StageLength"];
        result.Length = duration ?? 0.0f;
        result.LoopMode = Animation.LoopModeEnum.Linear;
        return result;
    }

    public void AddTrack(Animation anim, int uid, Node2D node, List<ActorState> states, ActorState? initial)
    {
        Debug.Assert(_animationPlayer != null);
        if (_animationPlayer == null) return;
        
        var totalStates = new List<ActorState>();
        if (initial != null)
        {
            initial.Time = 0.0f;
            totalStates.Add(initial);
        }
        states.Sort((a, b) => a.Time < b.Time ? -1 : a.Time > b.Time ? 1 : 0);
        totalStates.AddRange(states);
        
        //Add the final state at the end time point so godot doesnt interpolate the last
        //and first states together
        var finalState = new ActorState(states[^1])
        {
            Time = anim.Length
        };
        totalStates.Add(finalState);
        
        var centeredTrack = anim.AddTrack(Animation.TrackType.Value);
        var alignmentTrack = anim.AddTrack(Animation.TrackType.Value);
        var alphaTrack = anim.AddTrack(Animation.TrackType.Value);
        var angleTrack = anim.AddTrack(Animation.TrackType.Value);
        var colorTrack = anim.AddTrack(Animation.TrackType.Value);
        var posTrack = anim.AddTrack(Animation.TrackType.Value);
        var scaleTrack = anim.AddTrack(Animation.TrackType.Value);
        var shownTrack = anim.AddTrack(Animation.TrackType.Value);
        
        anim.TrackSetPath(centeredTrack, GetPathTo(node) + ":centered");
        anim.TrackSetPath(alignmentTrack, GetPathTo(node) + ":offset");
        anim.TrackSetPath(alphaTrack, GetPathTo(node) + ":Alpha");
        anim.TrackSetPath(angleTrack, GetPathTo(node) + ":rotation");
        anim.TrackSetPath(colorTrack, GetPathTo(node) + ":Color");
        anim.TrackSetPath(posTrack, GetPathTo(node) + ":position");
        anim.TrackSetPath(scaleTrack, GetPathTo(node) + ":scale");
        anim.TrackSetPath(shownTrack, GetPathTo(node) + ":visible");
        
        foreach (var state in totalStates)
        {
            var cell = state.Cell;
            var centerPoint = new Vector2(cell?.Aw ?? 0.0f, cell?.Ah ?? 0.0f) * 0.5f;
            var offset = Vector2.Zero;
            switch (state.Alignment[0])
            {
                case ActorAlignment.Default:
                    offset = new Vector2(cell?.Ax ?? 0.0f, offset.Y);
                    break;
                case ActorAlignment.MinX:
                    offset = new Vector2(cell?.Ax + (cell?.Aw * 0.5f) ?? 0.0f, offset.Y);
                    break;
                case ActorAlignment.MaxX:
                    offset = new Vector2(cell?.Ax - (cell?.Aw * 0.5f) ?? 0.0f, offset.Y);
                    break;
                case ActorAlignment.MinY:
                case ActorAlignment.MaxY:
                case ActorAlignment.Unknown3:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (state.Alignment[1])
            {
                case ActorAlignment.Default:
                    offset = new Vector2(offset.X, cell?.Ay ?? 0.0f);
                    break;
                case ActorAlignment.MinY:
                    offset = new Vector2(offset.X, cell?.Ay + (cell?.Ah * 0.5f) ?? 0.0f);
                    break;
                case ActorAlignment.MaxY:
                    offset = new Vector2(offset.X, cell?.Ay - (cell?.Ah * 0.5f) ?? 0.0f);
                    break;
                case ActorAlignment.MinX:
                case ActorAlignment.MaxX:
                case ActorAlignment.Unknown3:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            offset -= centerPoint;
            
            anim.TrackInsertKey(centeredTrack, state.Time, false);
            anim.TrackInsertKey(alignmentTrack, state.Time, offset);
            anim.TrackInsertKey(alphaTrack, state.Time, state.Alpha);
            anim.TrackInsertKey(angleTrack, state.Time, Mathf.DegToRad(state.Angle));
            anim.TrackInsertKey(colorTrack, state.Time, state.Color);
            anim.TrackInsertKey(posTrack, state.Time, state.Position * 4.0f);
            
            var scale = state.Scale;
            scale.X *= state.Flip is ActorFlip.Horizontal or ActorFlip.Both ? -1.0f : 1.0f;
            scale.Y *= state.Flip is ActorFlip.Vertical or ActorFlip.Both ? -1.0f : 1.0f;
            anim.TrackInsertKey(scaleTrack, state.Time, scale);
            
            anim.TrackInsertKey(shownTrack, state.Time, state.Shown);
        }
    }

    public Array<Node> GetActors() => GetChildren();
    
    public void Initialize()
    {
        foreach (var child in GetActors())
        {
            child.Free();
        }
        _animationPlayer?.Free();
        _animationPlayer = null;
        _animation = null;
        _usedCells.Clear();
        _initialStates.Clear();
        _childCells.Clear();

        if (SpriteDefinitionRes == "")
            return;
        var spriteDefinitionJson = JetFileImporter.Instance().GetJsonParsed(SpriteDefinitionRes);

        var animName = "Anim" + _animId;
        _animId++;
        
        _animationPlayer = new AnimationPlayer();
        AddChild(_animationPlayer);
        _animationPlayer.RootNode = GetPath();
        _animationPlayer.Name = "player";
        
        var stageOptions = spriteDefinitionJson["stageOptions"];
        if (stageOptions != null && stageOptions.ValueKind != JsonType.Null)
        {
            _usedCells = LoadSpriteInfo(stageOptions);
            _animation = LoadStageOptions(stageOptions);
        }

        var actors = spriteDefinitionJson["actors"] ??
                     throw new BTD5WouldCrashException("CompoundSprite file has no actors defined");
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
            foreach (var child in GetActors())
            {
                if (child is not Node2D n) continue;
                if (int.Parse(child.Name) != uid) continue;
            
                node = n;
                break;
            }
            Debug.Assert(node != null);
        
            AddTrack(_animation!, uid, node!, stages, _initialStates[uid]);
        }

        _animation!.ResourceName = animName;
        var library = new AnimationLibrary();
        library.AddAnimation(animName, _animation);
        _animationPlayer.AddAnimationLibrary("lib", library);
        Debug.Assert(_animationPlayer.HasAnimation("lib/" + animName));
        if(_animating)
            PlayAnimation();
        else
            PauseAnimation();
    }

    public bool Playing => _animationPlayer?.IsPlaying() ?? false;
    public void PlayAnimation()
    {
        _animationPlayer!.Play("lib/" + _animation!.ResourceName);
        _animating = true;
    }

    public void PauseAnimation()
    {
        if (_animationPlayer is not null)
            _animationPlayer!.Pause();
        _animating = false;
    }

    private void RefreshTextures()
    {
        foreach (var child in GetActors())
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
        if (FullyLoaded) return;
        FullyLoaded = _usedCells.All(cell => cell.GetTexture() != null);
        if (!FullyLoaded) return;
        
        RefreshTextures();
        Loaded?.Invoke(this, null!);
    }
}
