using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Collections;
using UnityEngine;

public class RecordableTarget : MonoBehaviour
{
    [Tooltip("Провайдеры значений, значения которых записываются по их ID при каждом тике (TODO: +'их'+) тикера")]
    [SerializeField] private List<RecordableValue> _recordableValues;

    [Tooltip("if null, use RecordingSystem._defaultTicker")]
    [SerializeField] private MonoTicker _defaultTicker;

    /// <summary>
    /// ID нужен для привязки записи к объекту во время его сериализации/десериализации.
    /// </summary>
    public int InstanceID;

    private RecordingSystem _recordingSystem;

    private RecordedTargetData _currentRecordingTargetData;

    public void StartRecording(RecordingSystem recordingSystem, RecordedGameData currentRecordOfGame)
    {
        _currentRecordingTargetData = new RecordedTargetData();
        currentRecordOfGame.AddRecordedTargetDataByInstanceID(_currentRecordingTargetData, InstanceID);

        _recordingSystem = recordingSystem;
        if (_defaultTicker == null) _defaultTicker = recordingSystem.DefaultTicker;

        foreach (var recordableValue in _recordableValues)
        {
            recordableValue.StartRecording(_currentRecordingTargetData, _defaultTicker, recordingSystem);
        }
    }

    public void StopRecording()
    {
        foreach (var recordableValue in _recordableValues)
        {
            recordableValue.StopRecording();
        }
    }
}
