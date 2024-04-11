using System;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using Godot;
using Godot.Collections;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;

public class ActorState
{
    private static readonly Shader ShaderResource = GD.Load<Shader>("res://Shaders/compound_sprite.tres");
    
    //Cell instance for retrieving alignment info and such
    private readonly CellEntry? _cellEntry = null;

    //Normal state stuff
    private ActorAlignment[] _alignment = new ActorAlignment[2];
    public float Alpha = 1.0f;
    private float _angle = 0.0f;
    public Color Color = Colors.White;
    private ActorFlip _flip = ActorFlip.Default;
    private Vector2 _position = Vector2.Zero;
    private Vector2 _scale = Vector2.One;
    private bool _shown = true;
    public float Time = 0.0f;

    public ActorState(CellEntry? cellEntry, JsonWrapper actor)
    {
        _cellEntry = cellEntry;
        if (actor.ValueKind == JsonType.Null)
            return;

        _alignment = new[]
        {
            actor["Alignment"]?[0].EnumValue<ActorAlignment>() ?? ActorAlignment.Default,
            actor["Alignment"]?[1].EnumValue<ActorAlignment>() ?? ActorAlignment.Default
        };
        Alpha = actor["Alpha"] ?? 1.0f;
        _angle = actor["Angle"] ?? 0.0f;
        if (actor.TryGetProperty("Colour", out var color))
        {
            var bytes = BitConverter.GetBytes(color.GetUInt32());
            Color.R = bytes[0] * 0.00390625f;
            Color.G = bytes[1] * 0.00390625f;
            Color.B = bytes[2] * 0.00390625f;
            Color.A = bytes[3] * 0.00390625f;
        }

        _flip = actor["Flip"]?.EnumValue<ActorFlip>() ?? ActorFlip.Default;
        _position = actor["Position"] ?? Vector2.Zero;
        _scale = actor["Scale"] ?? Vector2.One;
        _shown = actor["Shown"] ?? true;

        if (actor.TryGetProperty("Time", out var time))
        {
            Time = time.GetFloat();
        }
    }

    public void ApplyAndAlign(Node2D node)
    {
        if (node is Sprite sprite)
        {
            if (_cellEntry != null)
            {
                var centerPoint = new Vector2(_cellEntry.Aw, _cellEntry.Ah) * 0.5f;
                sprite.Centered = false;
                switch (_alignment[0])
                {
                    case ActorAlignment.Default:
                        sprite.Offset = new Vector2(_cellEntry.Ax, sprite.Offset.Y);
                        break;
                    case ActorAlignment.MinX:
                        sprite.Offset = new Vector2(_cellEntry.Ax + (_cellEntry.Aw * 0.5f), sprite.Offset.Y);
                        break;
                    case ActorAlignment.MaxX:
                        sprite.Offset = new Vector2(_cellEntry.Ax - (_cellEntry.Aw * 0.5f), sprite.Offset.Y);
                        break;
                    case ActorAlignment.MinY:
                    case ActorAlignment.MaxY:
                    case ActorAlignment.Unknown3:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                switch (_alignment[1])
                {
                    case ActorAlignment.Default:
                        sprite.Offset = new Vector2(sprite.Offset.X, _cellEntry.Ay);
                        break;
                    case ActorAlignment.MinY:
                        sprite.Offset = new Vector2(sprite.Offset.X, _cellEntry.Ay + (_cellEntry.Ah * 0.5f));
                        break;
                    case ActorAlignment.MaxY:
                        sprite.Offset = new Vector2(sprite.Offset.X, _cellEntry.Ay - (_cellEntry.Ah * 0.5f));
                        break;
                    case ActorAlignment.MinX:
                    case ActorAlignment.MaxX:
                    case ActorAlignment.Unknown3:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                sprite.Offset -= centerPoint;
            }
        }
        Apply(node);
    }
    public void ApplyColor(Sprite sprite)
    {
        sprite.Color = Color;
        sprite.Alpha = Alpha;
    }
    public void Apply(Node2D node)
    {
        var scale = _scale;
        node.Rotation = Mathf.DegToRad(_angle);
        node.Visible = _shown;
        node.Position = _position * 4;
        
        scale.X *= _flip is ActorFlip.Horizontal or ActorFlip.Both ? -1.0f : 1.0f;
        scale.Y *= _flip is ActorFlip.Vertical or ActorFlip.Both ? -1.0f : 1.0f;
        node.Scale = scale * (_cellEntry?.GetQualityScale() ?? 1.0f);
    }

    public void Interpolate(ActorState from, ActorState to, float delta)
    {
        if (from._cellEntry == null || to._cellEntry == null)
        {
            Debug.Assert(from._cellEntry == to._cellEntry);
        }
        else
        {
            Debug.Assert(from._cellEntry.CellName == to._cellEntry.CellName);
        }

        _alignment = from._alignment;
        Alpha = Mathf.Lerp(from.Alpha, to.Alpha, delta);
        _angle = Mathf.Lerp(from._angle, to._angle, delta);
        Color = from.Color.Lerp(to.Color, delta);
        _flip = from._flip;
        _position = from._position.Lerp(to._position, delta);
        _scale = from._scale.Lerp(to._scale, delta);
        _shown = from._shown;
        Time = Mathf.Lerp(from.Time, to.Time, delta);
    }
}