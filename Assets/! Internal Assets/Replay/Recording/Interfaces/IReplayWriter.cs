
public interface IReplayWriter<T>
{
    public void TakeSnapshot(T value);
    public IReplayReader<T> GetReplay();
}
