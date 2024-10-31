using System;

namespace BloonsTD5Rewritten.NewFramework.Scripts;

public interface IAsyncPromise
{
    public void FullfillPromise(object value);
    public void HandleError(Exception error);
}