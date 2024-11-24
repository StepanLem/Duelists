
using System;

public interface IRecordingController<T> : IPausable
{
    public bool IsPaused { get; }
    public IReplayReader<T> GetReplay();
}
