using System;

public interface IReplayReader<T>
{
    public bool TryRun(TimeSpan timeElapsed, out T value);
}