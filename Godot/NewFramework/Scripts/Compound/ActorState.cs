using System;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using Godot;
using Godot.Collections;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;

public class ActorState
{
    private static readonly Shader ShaderResource = GD.Load<Shader>("res://Godot/Shaders/compound_sprite.tres");
    
    //Cell instance for retrieving alignment info and such
    private readonly CellEntry? _cellEntry = null;

    //Normal state stuff
    private ActorAlignment[] _alignment = new ActorAlignment[2];
    private float _alpha = 1.0f;
    private float _angle = 0.0f;
    public Color Color = Colors.White;
    private ActorFlip _flip = ActorFlip.Default;
    private Vector2 _position = Vector2.Zero;
    private Vector2 _scale = Vector2.One;
    private bool _shown = true;
    public float Time = 0.0f;

    public ActorState(CellEntry? cellEntry, JsonElement actor)
    {
        _cellEntry = cellEntry;
        if (actor.ValueKind == JsonValueKind.Null)
            return;

        _alignment = new[]
        {
            (ActorAlignment)actor.GetProperty("Alignment")[0].GetInt32(),
            (ActorAlignment)actor.GetProperty("Alignment")[1].GetInt32()
        };
        _alpha = actor.GetProperty("Alpha").GetSingle();
        _angle = actor.GetProperty("Angle").GetSingle();
        if (actor.TryGetProperty("Colour", out var color))
        {
            var bytes = BitConverter.GetBytes(color.GetUInt32());
            Color.R = bytes[0] * 0.00390625f;
            Color.G = bytes[1] * 0.00390625f;
            Color.B = bytes[2] * 0.00390625f;
            Color.A = bytes[3] * 0.00390625f;
        }

        _flip = (ActorFlip)actor.GetProperty("Flip").GetInt32();
        _position = new Vector2(actor.GetProperty("Position")[0].GetSingle(), actor.GetProperty("Position")[1].GetSingle());
        _scale = new Vector2(actor.GetProperty("Scale")[0].GetSingle(), actor.GetProperty("Scale")[1].GetSingle());
        _shown = actor.GetProperty("Shown").GetBoolean();

        if (actor.TryGetProperty("Time", out var time))
        {
            Time = time.GetSingle();
        }
    }

    public void ApplyAndAlign(Node2D node)
    {
        if (node is Sprite2D sprite)
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
    public void ApplyColor(Node2D node)
    {
        if (node.Material == null)
        {
            var material = new ShaderMaterial();
            material.Shader = ShaderResource;
            node.Material = material;
        }
        (node.Material as ShaderMaterial)?.SetShaderParameter("color", Color);
    }
    public void Apply(Node2D node)
    {
        var scale = _scale;
        node.RotationDegrees = _angle;
        node.Visible = _shown;
        node.Position = _position * 4;
        
        scale.X *= _flip is ActorFlip.Horizontal or ActorFlip.Both ? -1.0f : 1.0f;
        scale.Y *= _flip is ActorFlip.Vertical or ActorFlip.Both ? -1.0f : 1.0f;
        node.Scale = scale;
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
        _alpha = Mathf.Lerp(from._alpha, to._alpha, delta);
        _angle = Mathf.Lerp(from._angle, to._angle, delta);
        Color = from.Color.Lerp(to.Color, delta);
        _flip = from._flip;
        _position = from._position.Lerp(to._position, delta);
        _scale = from._scale.Lerp(to._scale, delta);
        _shown = from._shown;
        Time = Mathf.Lerp(from.Time, to.Time, delta);
    }
}