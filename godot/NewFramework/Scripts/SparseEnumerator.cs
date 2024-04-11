using System;
using System.Collections;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public class SparseEnumerator<T> : IEnumerator
{
    public SparseEnumerator(SparseList<T> list)
    {
        _innerList = list;
        _cursor = -1;
    }
    
    public bool MoveNext()
    {
        if (_cursor < _innerList.Count)
            _cursor++;

        return _cursor != _innerList.Count;
    }

    public void Reset()
    {
        _cursor = -1;
    }

    public object? Current
    {
        get
        {
            if (_cursor < 0 || _cursor >= _innerList.Count)
                throw new InvalidOperationException();
            return _innerList[_cursor];
        }
    }

    private readonly SparseList<T> _innerList;
    private int _cursor;
}