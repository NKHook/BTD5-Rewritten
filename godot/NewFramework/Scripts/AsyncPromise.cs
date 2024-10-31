using System;

namespace BloonsTD5Rewritten.NewFramework.Scripts;

public class AsyncPromise<T> : IAsyncPromise where T : class
{
    public EventHandler<T>? Then;
    public EventHandler<Exception>? Error;

    public void FullfillPromise(object value)
    {
        Then?.Invoke(this, (value as T)!);
    }

    public void HandleError(Exception error)
    {
        Error?.Invoke(this, error);
    }
}