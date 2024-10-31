using System;

namespace BloonsTD5Rewritten.NewFramework.Scripts;

/// <summary>
/// Exception used to signify when the game would have normally crashed
/// </summary>
public class BTD5WouldCrashException : Exception
{
    public BTD5WouldCrashException(string? reason = "No reason provided") : base(reason)
    {
    }
}