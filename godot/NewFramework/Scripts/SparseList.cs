#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BloonsTD5Rewritten.NewFramework.Scripts;

public class SparseList<T> : IDictionary<int, T?>
{
    IEnumerator<KeyValuePair<int, T?>> IEnumerable<KeyValuePair<int, T?>>.GetEnumerator()
    {
        var i = 0;
        return _innerList.ToImmutableDictionary(val => i++).GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return new SparseEnumerator<T>(this);
    }

    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    public void Add(KeyValuePair<int, T?> item)
    {
        while (item.Key >= _innerList.Count)
            _innerList.Add(default);
        _innerList[item.Key] = item.Value;
    }

    public void Clear()
    {
        _innerList.Clear();
    }

    public bool Contains(KeyValuePair<int, T?> item)
    {
        return ContainsKey(item.Key) && (_innerList[item.Key]?.Equals(item.Value) ?? false);
    }

    public void CopyTo(KeyValuePair<int, T?>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<int, T?> item)
    {
        if (!Contains(item)) return Contains(item);
        
        _innerList[item.Key] = default!;
        return true;
    }

    public int Count => _innerList.Count;
    public bool IsReadOnly => false;
    public bool IsSynchronized => false;
    public object SyncRoot => this;

    private readonly List<T?> _innerList = new();
    public void Add(int key, T? value)
    {
        while (key >= _innerList.Count)
            _innerList.Add(default);
        _innerList[key] = value;
    }

    public bool ContainsKey(int key)
    {
        return key >= 0 && key < _innerList.Count && _innerList[key] != null;
    }

    public bool Remove(int key)
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(int key, out T? value)
    {
        value = default;
        if (!ContainsKey(key)) return false;
        
        value = _innerList[key];
        return true;
    }

    public T? this[int key]
    {
        get => _innerList.Count > key ? _innerList[key] : default;
        set
        {
            while (key >= _innerList.Count)
                _innerList.Add(default);
            _innerList[key] = value;
        }
    }

    public ICollection<int> Keys => Enumerable.Range(0, _innerList.Count).ToImmutableList();
    public ICollection<T?> Values => _innerList.Where(val => val is not null).ToImmutableList();
}