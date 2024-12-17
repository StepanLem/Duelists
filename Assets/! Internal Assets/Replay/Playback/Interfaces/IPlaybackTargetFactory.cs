using System;

public interface IPlaybackTargetFactory
{
    public IPlaybackTarget<T> CreateTarget<T>(Action<T> valueSetter);
}
