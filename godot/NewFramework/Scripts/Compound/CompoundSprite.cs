using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.NewFramework.Scripts.Sprites;
using Godot;
using FileAccess = Godot.FileAccess;

namespace BloonsTD5Rewritten.NewFramework.Scripts.Compound;

public partial class CompoundSprite : Node2D
{
	[ExportCategory("Compound Sprite")]
	[Export] public string SpriteDefinitionRes = "";
	[Export] public bool Animating = true;

	private bool _wasAnimating = false;
	private TimelineInterpolator? _timeline;
	private List<CellEntry> _usedCells = new();
	private readonly SparseList<ActorState> _initialStates = new();
	private readonly SparseList<CellEntry> _childCells = new();
	public Dictionary<string, CustomVariable> CustomVariables = new();
	public double PlaybackSpeed = 1.0;
	public IFileImporter DataSource = JetFileImporter.Instance();

	public EventHandler? Loaded = null;
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
		compoundSprite.DataSource = DataSource;
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
		SetSpriteCell(spriteObj, cell);
		state.ApplyAndAlign(spriteObj);
		state.ApplyColor(spriteObj);
		return spriteObj;
	}

	private Dictionary<string, CustomVariable> LoadCustomVariables(JsonWrapper variables)
	{
		var result = new Dictionary<string, CustomVariable>();
		foreach (var entry in variables.EnumerateArray())
		{
			var variable = new CustomVariable();
			var name = entry["VariableName"]!.GetString();
			variable.VariableName = name;
			variable.ValueType = entry["ValueType"]?.EnumValue<CustomVariableType>() ?? CustomVariableType.None;
			switch (variable.ValueType)
			{
				case CustomVariableType.None:
					break;
				case CustomVariableType.FloatArray:
					variable.Value = entry["Value"]?.ArrayAs<float>() ?? Array.Empty<float>();
					break;
				case CustomVariableType.IntArray:
					variable.Value = entry["Value"]?.ArrayAs<int>() ?? Array.Empty<int>();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			result[name] = variable;
		}

		return result;
	}
	
	private ActorNode? LoadActor(JsonWrapper actor)
	{
		var sprite = actor["sprite"] ?? "";
		Debug.Assert(sprite != string.Empty);
		var type = actor["type"]?.EnumValue<ActorTypes>() ?? ActorTypes.Invalid;
		var uid = actor["uid"]?.GetInt32() ?? -1;

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
		return new ActorNode(uid, result);
	}

	public static List<CellEntry> LoadSpriteInfo(JsonWrapper stageOptions)
	{
		var infosJson = stageOptions["SpriteInfo"];

		return (from info in infosJson!.EnumerateArray()
			let sprite = info["SpriteInfo"]
			let texture = info["Texture"]
			select TextureLoader.Instance()!.FindCell(sprite, texture).As<CellEntry>()
			into cell
			where cell != null
			select cell).ToList();
	}

	public static TimelineInterpolator LoadStageOptions(JsonWrapper stageOptions)
	{
		var duration = stageOptions["StageLength"]!;
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
		GD.Print($"Reading JSON: {DataSource.GetFileText(SpriteDefinitionRes)}");
		var spriteDefinitionJson = DataSource.GetJsonParsed(SpriteDefinitionRes);

		var customVarsJson = spriteDefinitionJson["customvariables"];
		if (customVarsJson != null && customVarsJson.ValueKind != JsonType.Null)
			CustomVariables = LoadCustomVariables(customVarsJson);
		
		var stageOptions = spriteDefinitionJson["stageOptions"];
		if (stageOptions != null && stageOptions.ValueKind != JsonType.Null)
		{
			_usedCells = LoadSpriteInfo(stageOptions);
			_timeline = LoadStageOptions(stageOptions);
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

			Node2D? node = GetActors().FirstOrDefault(actor => actor.SpriteUid == uid);
			Debug.Assert(node != null);
		
			_timeline?.AddTimeline(uid, node!, stages);
			_timeline?.SetInitialState(uid, _initialStates[uid]);
		}
	}

	private void RefreshTextures()
	{
		foreach (var actor in GetActors())
		{
			switch (actor.Node)
			{
				case Sprite sprite:
					SetSpriteCell(sprite, _childCells[actor.SpriteUid]);
					break;
				case CompoundSprite compound:
					compound.RefreshTextures();
					break;
			}
		}
	}

	public IEnumerable<ActorNode> GetActors() => GetChildren().OfType<ActorNode>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() => Initialize();

	public void StepTimeline(double delta)
	{
		if (Animating)
		{
			_timeline?.Tick((float)delta);
		}
		
		foreach (var actor in GetActors())
		{
			var state = _timeline?.GetStateForUid(actor.SpriteUid) ?? _initialStates[actor.SpriteUid];
			Debug.Assert(state is not null);
			
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (actor.Node is Sprite sprite && (state?.Color != sprite.Color || state?.Alpha != sprite.Alpha))
				state?.ApplyColor(sprite);
			state?.Apply(actor.Node);
		}
		
		if (FullyLoaded) return;
		FullyLoaded = _usedCells.All(cell => cell.GetTexture() != null);

		if (!FullyLoaded) return;
		
		RefreshTextures();
		Loaded?.Invoke(this, null!);
	}

	/*public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		StepTimeline(delta * PlaybackSpeed);
	}*/
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		StepTimeline(delta * PlaybackSpeed);
	}
}
