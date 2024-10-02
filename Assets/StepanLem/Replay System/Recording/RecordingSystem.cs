using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RecordingSystem : MonoBehaviour
{
    [SerializeField] private List<RecordableTarget> _recordableTargets;

    public MonoTicker DefaultTicker;

    public RecordedGameData currentRecordingData;

    public event Action<RecordedGameData> OnRecordingCompleted;

    [Tooltip("Last Recording Start Time in seconds relative to Game Start Time")]
    private float _lastRecordingStartTime;
    public float GetTimeFromRecordingStart() => Time.realtimeSinceStartup - _lastRecordingStartTime;

    public bool IsRecording { get; private set; }

    public void StartRecording()
    {
        if (IsRecording)
        {
            Debug.LogWarning("Запись уже идёт");
            return;
        }

        currentRecordingData = new();
        _lastRecordingStartTime = Time.realtimeSinceStartup;

        foreach (var recordableTarget in _recordableTargets)
        {
            recordableTarget.StartRecording(this, currentRecordingData);
        }

        IsRecording = true;
    }

    public void StopRecording()
    {
        if (!IsRecording)
        {
            Debug.LogWarning("Запись не велась");
            return;
        }

        foreach (var recordableTarget in _recordableTargets)
        {
            recordableTarget.StopRecording();
        }

        IsRecording = false;
        currentRecordingData.Duration = GetTimeFromRecordingStart();

        OnRecordingCompleted?.Invoke(currentRecordingData);
    }
}
