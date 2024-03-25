using System.Collections.Generic;
using System.Linq;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public class HashStringConverter
{
    //Useless since we can generate the string hash, but might be useful later so leaving this comment here...
    //private SparseList<Dictionary<string, int>> _hashLookup = new();
    private readonly SparseList<List<(uint, string)>> _stringLookup = new();

    public string HashToString(int category, uint hash)
    {
        if (!_stringLookup.TryGetValue(category, out var table)) return "invalid";
        var matches = table?.Where(entry => entry.Item1 == hash).ToList();
        if (matches?.Any() ?? false)
            return matches.First().Item2;
        return "invalid";
    }
    public uint StringToHash(int category, string text)
    {
        return text.Hash();
    }

    public void LoadCategory(int category, IEnumerable<string> types)
    {
        var typesList = types.ToList();
        var hashes = typesList.Select(type => StringToHash(category, type));
        _stringLookup[category] = hashes.Zip(typesList).ToList();
    }

    public bool HasCategory(int category)
    {
        return _stringLookup.ContainsKey(category);
    }
    public List<(uint, string)>? GetCategory(int category)
    {
        return _stringLookup[category];
    }

    public (uint, string)? GetEntry(int category, int entry)
    {
        return _stringLookup[category]?[entry];
    }
}