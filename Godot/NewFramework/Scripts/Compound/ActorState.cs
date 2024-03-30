using System;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using Godot;
using Godot.Collections;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;

public partial class ActorState
{
    private static readonly Shader ShaderResource = GD.Load<Shader>("res://Godot/Shaders/compound_sprite.tres");
    
    //Cell instance for retrieving alignment info and such
    private readonly CellEntry? _cellEntry = null;
    public CellEntry? Cell => _cellEntry;

    //Normal state stuff
    public ActorAlignment[] Alignment = new ActorAlignment[2];
    public float Alpha = 1.0f;
    public float Angle = 0.0f;
    public Color Color = Colors.White;
    public ActorFlip Flip = ActorFlip.Default;
    public Vector2 Position = Vector2.Zero;
    public Vector2 Scale = Vector2.One;
    public bool Shown = true;
    public float Time = 0.0f;

    public ActorState(ActorState other)
    {
        _cellEntry = other._cellEntry;
        Alignment = other.Alignment;
        Alpha = other.Alpha;
        Angle = other.Angle;
        Color = other.Color;
        Flip = other.Flip;
        Position = other.Position;
        Scale = other.Scale;
        Shown = other.Shown;
        Time = other.Time;
    }
    public ActorState(CellEntry? cellEntry, JsonWrapper actor)
    {
        _cellEntry = cellEntry;
        if (actor.ValueKind == JsonType.Null)
            return;

        Alignment = new[]
        {
            actor["Alignment"][0].EnumValue<ActorAlignment>(),
            actor["Alignment"][1].EnumValue<ActorAlignment>()
        };
        Alpha = actor["Alpha"];
        Angle = actor["Angle"];
        if (actor.TryGetProperty("Colour", out var color))
        {
            var bytes = BitConverter.GetBytes(color.GetUInt32());
            Color.R = bytes[0] / 255.0f;
            Color.G = bytes[1] / 255.0f;
            Color.B = bytes[2] / 255.0f;
            Color.A = bytes[3] / 255.0f;
        }

        Flip = actor["Flip"].EnumValue<ActorFlip>();
        Position = actor["Position"];
        Scale = actor["Scale"];
        Shown = actor["Shown"];

        if (actor.TryGetProperty("Time", out var time))
        {
            Time = time.GetFloat();
        }
    }

    public void Align(Node2D node)
    {
        if (node is not Sprite2D sprite) return;
        if (_cellEntry == null) return;
        
        var centerPoint = new Vector2(_cellEntry.Aw, _cellEntry.Ah) * 0.5f;
        sprite.Centered = false;
        switch (Alignment[0])
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
        switch (Alignment[1])
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
    
    public void ApplyAndAlign(Node2D node)
    {
        Align(node);
        Apply(node);
    }
    public void ApplyColor(Node2D node)
    {
        if (node is not Sprite sprite) return;
        
        sprite.Color = Color;
        sprite.Alpha = Alpha;
    }
    public void Apply(Node2D node)
    {
        var scale = Scale;
        node.RotationDegrees = Angle;
        node.Visible = Shown;
        node.Position = Position * 4;
        
        scale.X *= Flip is ActorFlip.Horizontal or ActorFlip.Both ? -1.0f : 1.0f;
        scale.Y *= Flip is ActorFlip.Vertical or ActorFlip.Both ? -1.0f : 1.0f;
        node.Scale = scale;
    }

    [Obsolete("Godot's animation system is being used now. You should never call this.")]
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

        Alignment = from.Alignment;
        Alpha = Mathf.Lerp(from.Alpha, to.Alpha, delta);
        Angle = Mathf.Lerp(from.Angle, to.Angle, delta);
        Color = from.Color.Lerp(to.Color, delta);
        Flip = from.Flip;
        Position = from.Position.Lerp(to.Position, delta);
        Scale = from.Scale.Lerp(to.Scale, delta);
        Shown = from.Shown;
        Time = Mathf.Lerp(from.Time, to.Time, delta);
    }
}