using System;

public interface IDataProvider<T> where T : struct
{
    public abstract T GetValue();

    public abstract void SetValue(T newValue);
}

public interface IReaderFactory //IPlaybackableDataProvider
{
    public IRecordReader CreateReader();
}

public interface IRecorderFactory //IRecordableDataProvider
{
    public IDataRecorder CreateRecorder(Func<float> recordingTimeGetter);
}