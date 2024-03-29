﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public class JsonWrapper : IEnumerable<(string, JsonWrapper)>, IEnumerable<JsonWrapper>
{
    private Variant _data;

    public JsonWrapper(Variant data)
    {
        _data = data;
    }

    /*public JsonWrapper(Variant data)
    {
        _data = data.Value;
    }*/

    public static implicit operator bool(JsonWrapper wrapper) => wrapper.Unwrap().AsBool();
    public static implicit operator short(JsonWrapper wrapper) => wrapper.Unwrap().AsInt16();
    public static implicit operator ushort(JsonWrapper wrapper) => wrapper.Unwrap().AsUInt16();
    public static implicit operator int(JsonWrapper wrapper) => wrapper.Unwrap().AsInt32();
    public static implicit operator uint(JsonWrapper wrapper) => wrapper.Unwrap().AsUInt32();
    public static implicit operator long(JsonWrapper wrapper) => wrapper.Unwrap().AsInt64();
    public static implicit operator ulong(JsonWrapper wrapper) => wrapper.Unwrap().AsUInt64();
    public static implicit operator float(JsonWrapper wrapper) => wrapper.Unwrap().AsSingle();
    public static implicit operator double(JsonWrapper wrapper) => wrapper.Unwrap().AsDouble();
    public static implicit operator string(JsonWrapper wrapper) => wrapper.Unwrap().AsString();
    public static implicit operator Vector2(JsonWrapper wrapper) => new(wrapper[0], wrapper[1]);
    public static implicit operator JsonWrapper[](JsonWrapper wrapper) => wrapper.EnumerateArray().ToArray();

    public bool GetBool() => Unwrap().AsBool();
    public short GetInt16() => Unwrap().AsInt16();
    public ushort GetUInt16() => Unwrap().AsUInt16();
    public int GetInt32() => Unwrap().AsInt32();
    public uint GetUInt32() => Unwrap().AsUInt32();
    public long GetInt64() => Unwrap().AsInt64();
    public ulong GetUInt64() => Unwrap().AsUInt64();
    public float GetFloat() => Unwrap().AsSingle();
    public double GetDouble() => Unwrap().AsDouble();
    public string GetString() => Unwrap().AsString();
    public Vector2 GetVector2() => new(this[0], this[1]);
    public JsonWrapper[] GetArray() => EnumerateArray().ToArray();

    public TFlag GetFlag<TFlag>() where TFlag : struct, Enum
    {
        if (ValueKind == JsonType.String)
        {
            return EnumName<TFlag>();
        }
        if (ValueKind == JsonType.Array && typeof(TFlag).GetEnumUnderlyingType() == typeof(ulong))
        {
            return (TFlag)(object)ArrayAs<string>().Select(text => Enum.Parse<TFlag>(text))
                .Aggregate<TFlag, ulong>(0, (current, val) => current | (ulong)(object)val);
        }

        throw new InvalidOperationException();
    }

    public T[] ArrayAs<T>() => EnumerateArray().Cast<T>().ToArray();

    public System.Collections.Generic.Dictionary<TK, TV> DictAs<TK, TV>() => EnumerateProperties()
        .ToDictionary(pair => (TK)(object)pair.Item1, pair => (TV)(object)pair.Item2);

    public int ArrayLen() => _data.AsGodotArray().Count;

    public TFlag EnumName<TFlag>() where TFlag : struct, Enum => Enum.Parse<TFlag>(_data.AsString());

    public T EnumValue<T>() where T : struct, Enum =>
        Enum.GetValues<T>().First(v => _data.AsInt32() == (int)(object)v);

    public JsonType ValueKind
    {
        get
        {
            switch (_data.VariantType)
            {
                case Variant.Type.Nil:
                    return JsonType.Null;
                case Variant.Type.Bool:
                    return JsonType.Bool;
                case Variant.Type.Int:
                    return JsonType.Integer;
                case Variant.Type.Float:
                    return JsonType.Number;
                case Variant.Type.StringName:
                case Variant.Type.String:
                    return JsonType.String;
                case Variant.Type.Vector2:
                case Variant.Type.Vector2I:
                    return JsonType.Vector2;
                case Variant.Type.Rect2:
                case Variant.Type.Rect2I:
                case Variant.Type.Vector3:
                case Variant.Type.Vector3I:
                case Variant.Type.Transform2D:
                case Variant.Type.Vector4:
                case Variant.Type.Vector4I:
                case Variant.Type.Plane:
                case Variant.Type.Quaternion:
                case Variant.Type.Aabb:
                case Variant.Type.Basis:
                case Variant.Type.Transform3D:
                case Variant.Type.Projection:
                case Variant.Type.Color:
                case Variant.Type.NodePath:
                case Variant.Type.Rid:
                case Variant.Type.Callable:
                case Variant.Type.Signal:
                case Variant.Type.Dictionary:
                case Variant.Type.Object:
                    return JsonType.Object;
                case Variant.Type.Array:
                    return JsonType.Array;
                case Variant.Type.PackedByteArray:
                case Variant.Type.PackedInt32Array:
                case Variant.Type.PackedInt64Array:
                case Variant.Type.PackedFloat32Array:
                case Variant.Type.PackedFloat64Array:
                case Variant.Type.PackedStringArray:
                case Variant.Type.PackedVector2Array:
                case Variant.Type.PackedVector3Array:
                case Variant.Type.PackedColorArray:
                case Variant.Type.Max:
                    return JsonType.Null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public bool TryGetProperty(string propertyName, out JsonWrapper data)
    {
        var dataDict = _data.AsGodotDictionary();
        if (dataDict?.TryGetValue(propertyName, out var result) ?? false)
        {
            data = new JsonWrapper(result);
            return true;
        }

        data = null!;
        return false;
    }

    /// <summary>
    /// Indexes a json object or array
    /// </summary>
    /// <param name="index">Index of the element or property</param>
    public JsonWrapper this[int index] => ValueKind == JsonType.Array
        ? new JsonWrapper(_data.AsGodotArray().ElementAt(index))
        : new JsonWrapper(_data.AsGodotObject()._GetPropertyList().ElementAt(index));

    /// <summary>
    /// Retrieves the property of an object
    /// </summary>
    /// <param name="property">The name of the property to retrieve</param>
    public JsonWrapper this[string property] => new(_data.AsGodotDictionary()[property]);

    /// <summary>
    /// Retrieve the underlying JSON element
    /// </summary>
    /// <returns>Underlying JsonElement</returns>
    public Variant Unwrap() => _data;

    /// <summary>
    /// Get an enumerator for the entries in an array
    /// </summary>
    /// <returns>An enumerator of JsonWrapper</returns>
    IEnumerator<JsonWrapper> IEnumerable<JsonWrapper>.GetEnumerator() => EnumerateArray().GetEnumerator();

    /// <summary>
    /// Get an enumerator for the object properties
    /// </summary>
    /// <returns>An enumerator of a tuple of a string for the property name and a JsonWrapper for the data.</returns>
    public IEnumerator<(string, JsonWrapper)> GetEnumerator() => EnumerateProperties().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerable<(string, JsonWrapper)> EnumerateProperties() =>
        _data.AsGodotDictionary().Select(dict => (dict.Key.AsString(), new JsonWrapper(dict.Value)));

    public IEnumerable<JsonWrapper> EnumerateArray() =>
        _data.AsGodotArray().Select(obj => new JsonWrapper(obj));

    public T Deserialize<T>() where T : new()
    {
        T result = new();
        foreach (var propInfo in typeof(T).GetProperties())
        {
            foreach (var (name, data) in EnumerateProperties())
            {
                if (propInfo.Name == name)
                {
                    propInfo.SetValue(result, data);
                }
            }
        }

        return result;
    }
}