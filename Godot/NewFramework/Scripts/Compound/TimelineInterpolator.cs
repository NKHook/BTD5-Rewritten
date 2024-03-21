#nullable enable
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;

public class TimelineInterpolator
{
    private float _time = 0.0f;
    private readonly float _length = 0.0f;
    private const bool Loop = true;
    private readonly SparseList<Node2D> _nodes = new();
    private readonly SparseList<List<ActorState>> _states = new();
    private readonly SparseList<ActorState> _currentStates = new();
    private readonly SparseList<ActorState> _initialStates = new();

    public TimelineInterpolator(float length)
    {
        _length = length;
    }

    public void Tick(float delta)
    {
        _time += delta;
        if (_time >= _length && Loop)
        {
            _time -= _time;
        }
    }

    public void AddTimeline(int uid, Node2D node, List<ActorState> states)
    {
        _nodes[uid] = node;
        
        states.Sort((a,b) => a.Time < b.Time ? -1 : a.Time > b.Time ? 1 : 0);
        _states[uid] = states;
        _currentStates[uid] = states.First();
    }

    public void SetInitialState(int uid, ActorState? state)
    {
        _initialStates[uid] = state;
    }

    public ActorState? GetStateForUid(int uid)
    {
        if (_states.Count == 0)
            return null;

        if (_states.Count <= uid || _states[uid] is null)
            return null;

        var actorStates = _states[uid];
        if (actorStates is null)
            return _initialStates[uid];
        
        var previousIdx = 0;
        ActorState? next = null;
        
        foreach (var i in Enumerable.Range(0, actorStates?.Count ?? 0))
        {
            if (actorStates?[i].Time <= _time)
            {
                previousIdx = i;
            }
            else
            {
                next = actorStates?[i];
                break;
            }
        }

        var previous = actorStates?[previousIdx].Time > _time ? _initialStates[uid] : actorStates?[previousIdx];

        if (next is null)
            return previous;
        if (next.Time < previous?.Time)
            return actorStates?.LastOrDefault();
        if (_time >= next.Time)
            return next;
        
        //Interpolate between the previous and next states
        var lerpFactor = (_time - previous?.Time) / (next.Time - previous?.Time) ?? 0.0f;
        _currentStates[uid]?.Interpolate(previous, next, lerpFactor);
        return _currentStates[uid];
    }
}