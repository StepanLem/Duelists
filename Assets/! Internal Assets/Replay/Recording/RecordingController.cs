using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class RecordingController<T> : IRecordingController<T>
{
    private readonly IReplayWriter<T> _recordingReel;
    private readonly Func<T> _valueGetter;
    private readonly ITicker _scheduler;

    public RecordingController(IReplayWriter<T> recordingReel, Func<T> valueGetter, ITicker ticker)
    {
        _recordingReel = recordingReel;
        _scheduler = ticker;
        _valueGetter = valueGetter;
    }

    public bool IsPaused { get; private set; } = true;

    private void TakeSnapshot()
    {
        _recordingReel.TakeSnapshot(_valueGetter.Invoke());
    }

    public void Start()
    {
        IsPaused = false;
        _scheduler.Fire += TakeSnapshot;
    }

    public void Stop()
    {
        IsPaused = false;
        _scheduler.Fire -= TakeSnapshot;
    }

    public IReplayReader<T> GetReplay()
    {
        return _recordingReel.GetReplay();
    }
}