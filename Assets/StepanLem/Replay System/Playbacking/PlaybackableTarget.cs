using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaybackableTarget : MonoBehaviour
{
    [SerializeField] private List<PlaybackableValue> _playbackableValues;

    [SerializeField] private MonoTicker _defaultTicker;

    /// <summary>
    /// ID нужен для привязки записи к объекту во время его сериализации/десериализации.
    /// </summary>
    public int InstanceID;

    private RecordedTargetData _currentPlaybackingTargetData;

    public event Action OnPlaybackCompleted;

    private int _countOfEndedPlaybackValues;

    public void StartPlaybacking(RecordedGameData recordedGameData, MonoTicker defaultTicker)
    {
        _currentPlaybackingTargetData = recordedGameData.GetRecordedTargetDataByInstanceID(InstanceID);

        if (_currentPlaybackingTargetData == null)
        {
            //Debug.Log("В RecordedData нет записи о объекте");
            OnPlaybackCompleted?.Invoke();
            return;
        }

        if (_playbackableValues.Count == 0)
        {
            Debug.LogWarning("У PlaybackableTarget нет ни одного PlaybackableValue. Нет чему выставлять значения.");
            OnPlaybackCompleted?.Invoke();
            return;
        }

        if (_defaultTicker == null) _defaultTicker = defaultTicker;
        _countOfEndedPlaybackValues = 0;

        foreach (var playbackableValue in _playbackableValues)
        {
            playbackableValue.StartPlaybacking(_currentPlaybackingTargetData, _defaultTicker);
            playbackableValue.OnPlaybackCompleted += OnValuePlaybackCompleted;
        }
    }

    private void OnValuePlaybackCompleted()
    {
        _countOfEndedPlaybackValues++;
        if (_countOfEndedPlaybackValues == _playbackableValues.Count)
            FinishPlaybacking();
    }

    public void FinishPlaybacking()
    {
        foreach (var playbackableValue in _playbackableValues)
        {
            playbackableValue.OnPlaybackCompleted -= OnValuePlaybackCompleted;
        }

        OnPlaybackCompleted?.Invoke();
    }
}
