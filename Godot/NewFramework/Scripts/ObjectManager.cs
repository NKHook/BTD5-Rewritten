using System.Collections.Generic;
using System.Linq;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public partial class ObjectManager<T> : Node2D where T : Node
{
	private Node2D? _container;
	public IEnumerable<T> Objects => _container?.GetChildren().AsEnumerable().Cast<T>() ?? Enumerable.Empty<T>();

	private int nextId = 0;

	public override void _Ready()
	{
		base._Ready();

		_container = new Node2D();
		_container.Name = "objects";
		AddChild(_container);
	}

	public int AddObject(T obj)
	{
		obj.Name = nextId.ToString();
		if (obj.IsInsideTree())
			obj.Reparent(_container);
		else
			_container?.AddChild(obj);
		nextId++;
		return nextId - 1;
	}

	public void RemoveObject(T obj)
	{
		_container?.RemoveChild(obj);
	}
}