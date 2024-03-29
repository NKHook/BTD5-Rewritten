using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public abstract partial class BaseFactory<TIdentifier, TInfo, TInstance> : Node2D
	where TIdentifier : struct, Enum
	where TInfo : class
	where TInstance : class
{
	protected string DefinitionsDir = "Assets/JSON/BaseFactory/";
	protected readonly FlagStringConverter TypeTracker = new();
	private readonly Dictionary<TIdentifier, TInfo> _factoryData = new();
	private readonly TInfo _invalid;

	protected BaseFactory(TInfo invalid)
	{
		_invalid = invalid;
	}

	protected abstract string ToFileName(string factoryName);

	public virtual TInfo GetInfo(string factoryName) => GetInfo(StringToFlag<TIdentifier>(factoryName));
	protected TInfo GetInfo(TIdentifier flag) => _factoryData.GetValueOrDefault(flag, _invalid);
	public virtual void AddInfo(TIdentifier id, TInfo info) => _factoryData[id] = info;

	public TFlag StringToFlag<TFlag>(string text) where TFlag : struct, Enum =>
		FlagStringConverter.StringToFlag<TFlag>(text);

	public string FlagToString<TFlag>(TFlag flag) where TFlag : struct, Enum =>
		FlagStringConverter.FlagToString(flag);

	public bool IsFlagProperty<TFlag>(string flagProperty) where TFlag : struct, Enum =>
		Enum.TryParse(flagProperty, out TFlag _);

	protected abstract TInfo GenerateInfo(JsonWrapper element);

	protected abstract void InitializeFactory();

	public virtual TInstance? Instantiate(string factoryName) => Instantiate(StringToFlag<TIdentifier>(factoryName));
	public virtual TInstance? Instantiate(TIdentifier flag) => Instantiate(GetInfo(flag));
	public TInstance? Instantiate(TInfo info) => Activator.CreateInstance(typeof(TInstance), info) as TInstance;
	
	public override void _Ready()
	{
		base._Ready();
		InitializeFactory();
	}
}