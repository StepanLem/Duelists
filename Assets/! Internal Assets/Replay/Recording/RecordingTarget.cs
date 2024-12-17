using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RecordingTarget<T> : IRecordingTarget<T>
{
    private readonly Func<T> _valueGetter;
    private readonly IInterpolator<T> _interpolator;
    private readonly IBinarySerializer<T> _serializer;
    public RecordingTarget(Func<T> valueGetter, IInterpolator<T> interpolator, IBinarySerializer<T> serializer)
    {
        _valueGetter = valueGetter;
        _interpolator = interpolator;
        _serializer = serializer;
    }
    public IRecordingController<T> StartRecording(ITicker ticker)
    {
        var reel = new ReplayWriter<T>(_interpolator);
        var controller = new RecordingController<T>(reel, _valueGetter, ticker);
        controller.Start();
        return controller;
    }
}