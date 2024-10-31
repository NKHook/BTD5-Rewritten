using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace BloonsTD5Rewritten.NewFramework.Scripts;

public class JsonWrapper : IEnumerable<(string, JsonWrapper)>, IEnumerable<JsonWrapper>
{
    private Variant _data;

    public JsonWrapper()
    {
        _data = new Variant();
    }

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

    public Color GetColor()
    {
        if (ValueKind == JsonType.Array && ArrayLen() > 0)
        {
            if (ElementKind == JsonType.Integer)
            {
                var valInts = new[] { 255, 255, 255, 255 };
                var ints = ArrayAs<int>();
                for (var i = 0; i < ints.Length; i++)
                {
                    valInts[i] = ints[i];
                }

                return new Color(valInts[0] / 255.0f, valInts[1] / 255.0f, valInts[2] / 255.0f, valInts[3] / 255.0f);
            }

            if (ElementKind == JsonType.Number)
            {
                var valSingles = new[] { 1.0f, 1.0f, 1.0f, 1.0f };
                var singles = ArrayAs<float>();
                for (var i = 0; i < singles.Length; i++)
                {
                    valSingles[i] = singles[i];
                }

                return new Color(singles[0], singles[1], singles[2], singles[3]);
            }
        }

        if (ValueKind == JsonType.Integer)
        {
            var result = Colors.White;
            var bytes = BitConverter.GetBytes(GetUInt32());
            result.R = bytes[0] * 0.00390625f;
            result.G = bytes[1] * 0.00390625f;
            result.B = bytes[2] * 0.00390625f;
            result.A = bytes[3] * 0.00390625f;
            return result;
        }
        
        return Colors.White;
    }
    public JsonWrapper[] GetArray() => EnumerateArray().ToArray();

    public T ValueAs<T>()
    {
        if (typeof(T) == typeof(bool))
        {
            return (T)(object)GetBool();
        }

        if (typeof(T) == typeof(short))
        {
            return (T)(object)GetInt16();
        }

        if (typeof(T) == typeof(ushort))
        {
            return (T)(object)GetUInt16();
        }

        if (typeof(T) == typeof(int))
        {
            return (T)(object)GetInt32();
        }

        if (typeof(T) == typeof(uint))
        {
            return (T)(object)GetUInt32();
        }

        if (typeof(T) == typeof(long))
        {
            return (T)(object)GetInt64();
        }

        if (typeof(T) == typeof(ulong))
        {
            return (T)(object)GetUInt64();
        }

        if (typeof(T) == typeof(float))
        {
            return (T)(object)GetFloat();
        }

        if (typeof(T) == typeof(double))
        {
            return (T)(object)GetDouble();
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)GetString();
        }

        if (typeof(T) == typeof(Vector2))
        {
            return (T)(object)GetVector2();
        }

        if (typeof(T) == typeof(Color))
        {
            return (T)(object)GetColor();
        }
        
        if (typeof(T) == typeof(JsonWrapper[]))
        {
            return (T)(object)GetArray();
        }

        throw new InvalidCastException("No available conversion");
    }

    public TFlag GetFlag<TFlag>() where TFlag : struct, Enum
    {
        if (ValueKind == JsonType.String)
        {
            return EnumName<TFlag>();
        }

        if (ValueKind == JsonType.Integer || ValueKind == JsonType.Number)
        {
            return (TFlag)(object)GetInt32();
        }

        if (ValueKind == JsonType.Array && typeof(TFlag).GetEnumUnderlyingType() == typeof(ulong))
        {
            return (TFlag)(object)ArrayAs<string>().Select(text => Enum.Parse<TFlag>(text))
                .Aggregate<TFlag, ulong>(0, (current, val) => current | (ulong)(object)val);
        }
        if (typeof(TFlag).GetEnumUnderlyingType() != typeof(ulong))
        {
            throw new InvalidOperationException("The underlying type of a flag must be a ulong");
        }
        throw new InvalidOperationException();
    }

    public T[] ArrayAs<T>()
    {
        if (!typeof(T).IsArray)
            return EnumerateArray().Select(entry => entry.ValueAs<T>()).ToArray();
        
        var thisMethod = typeof(JsonWrapper).GetMethod("ArrayAs");
        var compiled = thisMethod!.MakeGenericMethod(typeof(T).GetElementType()!);
        return EnumerateArray().Select(entry => compiled.Invoke(entry, null)).Cast<T>().ToArray();
    }

    public Dictionary<TK, TV> DictAs<TK, TV>() where TK : notnull => EnumerateProperties()
        .ToDictionary(pair => (TK)(object)pair.Item1, pair => pair.Item2.ValueAs<TV>());

    public int ArrayLen() => _data.AsGodotArray().Count;

    public TFlag EnumName<TFlag>() where TFlag : struct, Enum =>
        _data.AsString() != string.Empty ? Enum.Parse<TFlag>(_data.AsString()) : default;

    public T EnumValue<T>() where T : struct, Enum =>
        Enum.GetValues<T>().First(v => _data.AsInt32() == (int)(object)v);

    public JsonType ElementKind
    {
        get
        {
            if (ValueKind == JsonType.Array)
            {
                if (ArrayLen() <= 0)
                    return JsonType.Null;

                var resultKind = JsonType.Null;
                foreach (var val in EnumerateArray())
                {
                    switch (val.ValueKind)
                    {
                        case JsonType.Array:
                            if(resultKind == JsonType.Null)
                                resultKind = JsonType.Array;
                            else if (resultKind != JsonType.Array)
                                resultKind = JsonType.Object;
                            break;
                        case JsonType.Object:
                            if(resultKind == JsonType.Null)
                                resultKind = JsonType.Object;
                            break;
                        case JsonType.Null:
                            break;
                        case JsonType.Vector2:
                            if(resultKind == JsonType.Null)
                                resultKind = JsonType.Vector2;
                            else if (resultKind != JsonType.Vector2)
                                resultKind = JsonType.Object;
                            break;
                        case JsonType.String:
                            if(resultKind == JsonType.Null)
                                resultKind = JsonType.String;
                            else if (resultKind != JsonType.String)
                                resultKind = JsonType.Object;
                            break;
                        case JsonType.Integer:
                            if(resultKind == JsonType.Null)
                                resultKind = JsonType.Integer;
                            else if (resultKind != JsonType.Integer)
                                resultKind = JsonType.Object;
                            break;
                        case JsonType.Number:
                            if(resultKind == JsonType.Null)
                                resultKind = JsonType.Number;
                            else if (resultKind == JsonType.Integer)
                                resultKind = JsonType.Number;
                            else if (resultKind != JsonType.Number)
                                resultKind = JsonType.Object;
                            break;
                        case JsonType.Bool:
                            if(resultKind == JsonType.Null)
                                resultKind = JsonType.Bool;
                            else if (resultKind != JsonType.Bool)
                                resultKind = JsonType.Object;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return resultKind;
            }

            return JsonType.Null;
        }
    }
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
        if (dataDict.TryGetValue(propertyName, out var result))
        {
            data = new JsonWrapper(result);
            return true;
        }

        var dataObj = _data.AsGodotObject();
        var propData = dataObj?.Get(propertyName);
        if (propData != null && propData.Value.VariantType != Variant.Type.Nil)
        {
            data = new JsonWrapper(propData.Value);
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
    public JsonWrapper? this[string property] => TryGetProperty(property, out var data) ? data : null;

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