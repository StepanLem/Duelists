using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class PlaybackController<T> : IPlaybackController
{
    private readonly ITicker _ticker;
    private readonly IReplayReader<T> _replayReader;
    private readonly Action<T> _valueSetter;

    public PlaybackController(Action<T> valueSetter, IReplayReader<T> replayReader, ITicker ticker)
    {
        _ticker = ticker;
        _replayReader = replayReader;
        _valueSetter = valueSetter;
    }

    public event Action OnFinish;

    public void Start()
    {
        _ticker.Fire += Update;
    }

    public void Stop()
    {
        _ticker.Fire -= Update;
    }

    private void Update()
    {
        var dt = TimeSpan.FromSeconds(Time.deltaTime);
        if (_replayReader.TryRun(dt, out var value))
        {
            _valueSetter?.Invoke(value);
        }
        else
        {
            Stop();
            OnFinish?.Invoke();
        }
    }
}
