using System;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;

public interface IResourcePromise
{
    public void FullfillPromise(object value);
    public void HandleError(Exception error);
}