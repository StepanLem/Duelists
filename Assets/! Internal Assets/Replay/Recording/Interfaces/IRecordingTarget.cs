
public interface IRecordingTarget<T>
{
    public IRecordingController<T> StartRecording(ITicker ticker);
}
