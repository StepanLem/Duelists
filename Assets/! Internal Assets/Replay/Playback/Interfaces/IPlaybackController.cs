
using System;

public interface IPlaybackController : IPausable
{
    public event Action OnFinish;
}