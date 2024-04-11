using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using Godot;

namespace BloonsTD5Rewritten.NewFramework.Scripts;

public partial class ObjectManager<T> : Node2D where T : Node, IManagedObject
{
	private Node2D? _container;
	public IEnumerable<T> Objects => _container?.GetChildren().AsEnumerable().Cast<T>() ?? Enumerable.Empty<T>();
	public int Count => _container?.GetChildCount() ?? 0;

	private int nextId = 0;

	public override void _Ready()
	{
		base._Ready();

		_container = new Node2D();
		_container.Name = "objects";
		AddChild(_container);
	}

	public virtual int AddObject(T obj)
	{
		obj.OwnedBy(this);
		obj.Name = nextId.ToString();
		if (obj.IsInsideTree())
			obj.Reparent(_container);
		else
			_container?.AddChild(obj);
		nextId++;
		return nextId - 1;
	}

	public virtual void RemoveObject(T obj)
	{
		_container?.RemoveChild(obj);
	}
}