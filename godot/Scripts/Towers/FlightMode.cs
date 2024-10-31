namespace BloonsTD5Rewritten.Scripts.Towers;

public enum FlightMode
{
    FollowPath = 1 << 1,
    Patrol = 1 << 2,
    FollowPlayer = 1 << 3,
    LockInPlace = 1 << 4,
    Pursuit = 1 << 5
}