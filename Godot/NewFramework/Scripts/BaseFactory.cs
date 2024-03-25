using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public abstract partial class BaseFactory<TInfo, TInstance> : Node2D
	where TInfo : class
	where TInstance : class
{
	protected string DefinitionsDir = "Assets/JSON/BaseFactory/";
	protected readonly HashStringConverter TypeTracker = new();
	private readonly Dictionary<uint, TInfo> _factoryData = new();
	private readonly TInfo _invalid;

	protected BaseFactory(TInfo invalid)
	{
		_invalid = invalid;
	}

	protected abstract string ToFileName(string factoryName);

	public TInfo GetInfo(string factoryName) => GetInfo(HashName(0, factoryName));
	public TInfo GetInfo(uint hash) => _factoryData.GetValueOrDefault(hash, _invalid);

	public uint HashName(int category, string factoryName)
	{
		return TypeTracker.StringToHash(category, factoryName);
	}

	protected abstract TInfo GenerateInfo(JsonElement element);

	public TInstance? Instantiate(string factoryName) => Instantiate(HashName(0, factoryName));
	public TInstance? Instantiate(uint hash) => Instantiate(GetInfo(hash));
	public TInstance? Instantiate(TInfo info) => Activator.CreateInstance(typeof(TInstance), info) as TInstance;
	
	public override void _Ready()
	{
		base._Ready();
		var fileImporter = JetFileImporter.Instance();

		var factoryHashes = TypeTracker
			.GetCategory(0)?
			.Select(pair => pair.Item1)
			.ToList();
		var factoryNames = TypeTracker
			.GetCategory(0)?
			.Select(pair => pair.Item2)
			.ToList();
		var infos = factoryNames?
			.Select(ToFileName)
			.Select(name => DefinitionsDir + name)
			.Select(path => fileImporter.GetJsonParsed(path))
			.Select(GenerateInfo);

		Debug.Assert(factoryNames != null);
		Debug.Assert(infos != null);
		
		foreach (var (hash, info) in factoryHashes!.Zip(infos!))
		{
			_factoryData[hash] = info;
		}
	}
}