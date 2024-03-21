using System;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;

public class ResourcePromise<T> : IResourcePromise where T : class
{
    public EventHandler<T> Then = null;
    public EventHandler<Exception> Error = null;

    public void FullfillPromise(object value)
    {
        Then?.Invoke(this, value as T);
    }

    public void HandleError(Exception error)
    {
        Error?.Invoke(this, error);
    }
}