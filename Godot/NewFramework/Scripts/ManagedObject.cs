using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public interface IManagedObject
{
    public void OwnedBy(object? owner);
}