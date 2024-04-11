using System;
using System.Collections.Generic;
using System.Linq;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public static class Flags
{
    public static IEnumerable<T> ForEach<T>(this T val) where T : struct, Enum =>
        from flag in Enum.GetValues<T>() where val.HasFlag(flag) select val;

    public static T FirstOrDefault<T>(this T val) where T : struct, Enum =>
        Enum.GetValues<T>().FirstOrDefault(e => val.HasFlag(val));
}