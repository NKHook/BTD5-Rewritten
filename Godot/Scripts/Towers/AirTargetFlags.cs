namespace BloonsTD5Rewritten.Godot.Scripts.Towers;

public enum AirTargetFlags
{
    FollowPath = 1 << 1,
    Patrol = 1 << 2,
    FollowPlayer = 1 << 3,
    LockInPlace = 1 << 4,
    Pursuit = 1 << 5
}