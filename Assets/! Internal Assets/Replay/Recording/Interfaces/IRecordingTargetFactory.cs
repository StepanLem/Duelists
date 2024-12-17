using System;

public interface IRecordingTargetFactory
{
    public IRecordingTarget<T> CreateTarget<T>(Func<T> valueGetter);
}
