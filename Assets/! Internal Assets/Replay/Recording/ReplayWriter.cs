using System;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class ReplayWriter<T> : IReplayWriter<T>
{
    private readonly IInterpolator<T> _interpolator;
    private readonly List<Snapshot<T>> _frames = new ();

    private T _cachedValue;
    private DateTime _cachedTime;
    private int _skipCounter;

    public ReplayWriter(IInterpolator<T> interpolator)
    {
        _interpolator = interpolator;
    }

    public IReplayReader<T> GetReplay()
    {
        DropCachedFrame();
        return new ReplayReader<T>(_interpolator, _frames);
    }

    public void TakeSnapshot(T value)
    {
        if (value.Equals(_cachedValue))
        {
            _skipCounter++;
            _cachedTime = DateTime.UtcNow;
            return;
        }
        else
        {
            DropCachedFrame();
            _cachedValue = value;
        }
        AddFrame(value, DateTime.UtcNow);
    }

    private void DropCachedFrame()
    {
        if (_skipCounter == 0)
        {
            return;
        }
        AddFrame(_cachedValue, _cachedTime);
        _skipCounter = 0;
    }

    private void AddFrame(T value, DateTime dateTime)
    {
        _frames.Add(new Snapshot<T>(value, dateTime));
    }
}