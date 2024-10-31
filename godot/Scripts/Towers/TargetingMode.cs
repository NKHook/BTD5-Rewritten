using System;

namespace BloonsTD5Rewritten.Scripts.Towers;

[Flags]
public enum TargetingMode
{
    First = 1 << 0,
    Last = 1 << 1,
    Close = 1 << 2,
    Strong = 1 << 3,
    AnyInRange = 1 << 4,
    Any = 1 << 5,
    ManualActive = 1 << 6,
    ManualPassive = 1 << 7,
    Arc = 1 << 8,
    None = 1 << 9,
    Submerge = 1 << 10
}