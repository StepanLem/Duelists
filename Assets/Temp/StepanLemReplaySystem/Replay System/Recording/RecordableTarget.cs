using System.Collections.Generic;
using UnityEngine;

public class RecordableTarget : MonoBehaviour
{
    [SerializeField] private List<RecordableValue> _recordableValues;

    [Tooltip("Нужен для привязки записи к объекту во время его сериализации/десериализации.")]
    public int TargetDataID;

    [Tooltip("if null, use RecordingSystem._defaultTicker")]
    [SerializeField] private MonoTicker _defaultTicker;
    private bool _usedDefaultTicker;

    public void StartRecording(RecordedGameData currentRecordedGameData, MonoTicker defaultTicker, IRecordingDurationProvider recordingDurationProvider)
    {
        var currentRecordedTargetData = new RecordedTargetData();
        currentRecordedGameData.AddRecordedTargetDataByInstanceID(currentRecordedTargetData, TargetDataID);

        if (_defaultTicker == null) { _defaultTicker = defaultTicker; _usedDefaultTicker = true;  }

        foreach (var recordableValue in _recordableValues)
        {
            recordableValue.StartRecording(currentRecordedTargetData, _defaultTicker, recordingDurationProvider);
        }
    }

    public void StopRecording()
    {
        foreach (var recordableValue in _recordableValues)
        {
            recordableValue.StopRecording();
        }

        if (_usedDefaultTicker) { _defaultTicker = null; _usedDefaultTicker = false; }
    }
}
