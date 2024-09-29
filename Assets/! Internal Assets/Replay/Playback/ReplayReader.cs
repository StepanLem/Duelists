using System;
using System.Collections.Generic;

public class ReplayReader<T> : IReplayReader<T>
{
    readonly struct Transition
    {
        public readonly T Initial;
        public readonly T Final;
        public readonly DateTime Starts;
        public readonly DateTime Ends;

        public Transition(T initial, T final, DateTime starts, DateTime ends)
        {
            Initial = initial;
            Final = final;
            Starts = starts;
            Ends = ends;
        }
        public TimeSpan Duration => Ends - Starts;
    }

    private readonly IEnumerator<Snapshot<T>> _enumerator;
    private readonly IInterpolator<T> _interpolator;

    public ReplayReader(IInterpolator<T> interpolator, IEnumerable<Snapshot<T>> snapshots)
    {
        _interpolator = interpolator;
        _enumerator = snapshots.GetEnumerator();
    }

    private TimeSpan FrameTimeElapsed {  get; set; }
    private Transition? Current {  get; set; }

    private bool TryStart()
    {
        if (!_enumerator.MoveNext())
        {
            return false;
        }

        var start = _enumerator.Current;

        if (!_enumerator.MoveNext())
        {
            return false;
        }
        var end = _enumerator.Current;
        Current = new Transition(start.Value, end.Value, start.TimeStamp, end.TimeStamp);
        return true;
    }

    private bool TryAdvance()
    {
        if (_enumerator.MoveNext())
        {
            Current = new Transition(
                Current.Value.Final, 
                _enumerator.Current.Value, 
                Current.Value.Ends, 
                _enumerator.Current.TimeStamp);
            return true;
        }
        return false;
    }

    public bool TryRun(TimeSpan timeElapsed, out T value)
    {
        if (!Current.HasValue)
        {
            if (!TryStart())
            {
                value = default;
                return false;
            }
        }

        FrameTimeElapsed += timeElapsed;
        if (FrameTimeElapsed > Current.Value.Duration)
        {
            FrameTimeElapsed -= Current.Value.Duration;
            if (!TryAdvance())
            {
                value = Current.Value.Final;
                return false;
            }
        }
        value = _interpolator.Lerp(
            Current.Value.Initial, 
            Current.Value.Final, 
            timeElapsed / Current.Value.Duration);

        return true;
    }
}