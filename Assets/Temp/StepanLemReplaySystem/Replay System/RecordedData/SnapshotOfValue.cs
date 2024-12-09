public readonly struct Snapshots<T>
{
    public Snapshots(T value, float time)
    {
        Value = value;
        Time = time;
    }

    public readonly T Value;
    public readonly float Time;
}

