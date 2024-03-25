using System;
using System.Collections.Generic;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public static class TypedHashStringConverter
{
    public static string HashToString<TCateogryEnum>(this HashStringConverter self, TCateogryEnum category, uint hash)
        where TCateogryEnum : Enum => self.HashToString((int)(object)category, hash);

    public static uint StringToHash<TCateogryEnum>(this HashStringConverter self, TCateogryEnum category, string text)
        where TCateogryEnum : Enum => self.StringToHash((int)(object)category, text);

    public static void LoadCategory<TCateogryEnum>(this HashStringConverter self, TCateogryEnum category,
        IEnumerable<string> types) where TCateogryEnum : Enum => self.LoadCategory((int)(object)category, types);

    public static bool HasCategory<TCateogryEnum>(this HashStringConverter self, TCateogryEnum category)
        where TCateogryEnum : Enum => self.HasCategory((int)(object)category);

    public static List<(uint, string)>? GetCategory<TCateogryEnum>(this HashStringConverter self,
        TCateogryEnum category) where TCateogryEnum : Enum => self.GetCategory((int)(object)category);

    public static (uint, string)? GetEntry<TCategoryEnum>(this HashStringConverter self, TCategoryEnum category,
        int entry) where TCategoryEnum : Enum => self.GetEntry((int)(object)category, entry);
}