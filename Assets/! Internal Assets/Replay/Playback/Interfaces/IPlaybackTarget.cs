using System;

public interface IPlaybackTarget<T>
{
    public IPlaybackController StartPlayback(IReplayReader<T> replayReader, ITicker ticker);
}
