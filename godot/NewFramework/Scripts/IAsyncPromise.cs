using System;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;

public interface IAsyncPromise
{
    public void FullfillPromise(object value);
    public void HandleError(Exception error);
}