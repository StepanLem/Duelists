using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlaybackTarget<T> : IPlaybackTarget<T>
{
    private Action<T> _valueSetter;
    public PlaybackTarget(Action<T> valueSetter)
    {
        _valueSetter = valueSetter;
    }
    public IPlaybackController StartPlayback(IReplayReader<T> replayReader, ITicker ticker)
    {
        var controller = new PlaybackController<T>(_valueSetter, replayReader, ticker);
        controller.Start();
        return controller;
    }
}