using System;
using Godot;

namespace BloonsTD5Custom.Godot.Scripts;

internal static class Debug
{
    internal static void Assert(bool cond, string msg = "No message provided.")
#if DEBUG
    {
        if (cond) return;

        GD.PrintErr(msg);
        throw new ApplicationException($"Assert Failed: {msg}");
    }
#else
    {}
#endif
}