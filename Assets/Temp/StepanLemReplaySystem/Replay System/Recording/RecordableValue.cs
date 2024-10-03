using UnityEngine;

public class RecordableValue : MonoBehaviour
{
    [Tooltip("Передаёт значение для записи его данных в RecordedData по ValueDataID.")]
    [SerializeField] private IValueProvider _valueProvider;

    [Tooltip("Нужен для привязки записи к объекту во время его сериализации/десериализации.")]
    public int ValueDataID;

    [Tooltip("If null, use defaultTicker")]
    [SerializeField] private MonoTicker _ticker;
    private bool _usedDefaultTicker;

    private RecordedValueData _currentRecordedValueData;
    private IRecordingDurationProvider _recordingDurationProvider;

    public void StartRecording(RecordedTargetData _currentRecordedTargetData, MonoTicker defaultTicker, IRecordingDurationProvider recordingDurationProvider)
    {
        _currentRecordedValueData = new RecordedValueData();
        _currentRecordedTargetData.AddRecordedValueDataByInstanceID(_currentRecordedValueData, ValueDataID);

        if (_ticker == null) { _ticker = defaultTicker; _usedDefaultTicker = true;  }

        _recordingDurationProvider = recordingDurationProvider;

        _ticker.Fire += MakeSnapshot;
    }

    public void StopRecording()
    {
        _ticker.Fire -= MakeSnapshot;

        _currentRecordedValueData = null;
        if (_usedDefaultTicker) { _ticker = null; _usedDefaultTicker = false; }
        _recordingDurationProvider = null;
    }

    public void MakeSnapshot()
    {
        _currentRecordedValueData.Snapshots.Add(new SnapshotOfValue(_valueProvider.GetValue(), _recordingDurationProvider.GetCurrentRecordingDuration()));
    }
}
