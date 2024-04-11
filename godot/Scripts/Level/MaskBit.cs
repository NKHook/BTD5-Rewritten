using System;

namespace BloonsTD5Rewritten.Godot.Scripts.Level;

[Flags]
public enum MaskBit : byte
{
    BlockTower = 1 << 0,
    PathTower = 1 << 1,
    Water = 1 << 2
}