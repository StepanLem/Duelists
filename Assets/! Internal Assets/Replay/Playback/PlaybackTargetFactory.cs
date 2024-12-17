using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlaybackTargetFactory : IPlaybackTargetFactory
{
    public IPlaybackTarget<T> CreateTarget<T>(Action<T> valueSetter)
    {
        return new PlaybackTarget<T>(valueSetter);
    }
}