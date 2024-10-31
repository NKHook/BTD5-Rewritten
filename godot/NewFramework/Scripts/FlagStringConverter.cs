using System;

namespace BloonsTD5Rewritten.NewFramework.Scripts;

public class FlagStringConverter
{
    public static string FlagToString<TFlag>(TFlag flag) where TFlag : Enum
    {
        return flag.ToString();
    }
    public static TFlag StringToFlag<TFlag>(string text) where TFlag : struct, Enum
    {
        return Enum.TryParse(text, out TFlag flag) ? flag : default;
    }
}