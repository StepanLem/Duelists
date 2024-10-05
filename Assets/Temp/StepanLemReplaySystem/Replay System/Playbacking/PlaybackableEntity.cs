using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackableEntity : MonoBehaviour
{
    [SerializeField] private List<PlaybackerComponent> _playbackers;

    [Tooltip("Нужен для привязки записи к объекту во время его сериализации/десериализации.")]
    public int TargetDataID;

    [Tooltip("if null, use PlaybackingSystem._defaultTicker")]
    [SerializeField] private MonoTicker _defaultTicker;
    private bool _usedDefaultTicker;

    public event Action OnPlaybackCompleted;

    private int _countOfCompletedPlaybackers;

    public void StartPlaybacking(GameRecord recordedGameData, MonoTicker defaultTicker)
    {
        var currentPlaybackingEntityData = recordedGameData.GetEntityRecordByID(TargetDataID);

        //Обработки на null
        /*if (currentPlaybackingEntityData == null)
        {
            //Debug.Log("В RecordedData нет записи о объекте");
            OnPlaybackCompleted?.Invoke();
            return;
        }

        if (_playbackers.Count == 0)
        {
            Debug.LogWarning("У PlaybackableEntity нет ни одного PlaybackerComponent. Нет чему выставлять значения.");
            OnPlaybackCompleted?.Invoke();
            return;
        }*/

        _countOfCompletedPlaybackers = 0;
        if (_defaultTicker == null) { _defaultTicker = defaultTicker; _usedDefaultTicker = true; }
        
        foreach (var paybackableComponent in _playbackers)
        {
            paybackableComponent.OnPlaybackCompleted += OnPlaybackerCompleted;
            paybackableComponent.StartPlaybacking(currentPlaybackingEntityData, _defaultTicker);
        }
    }

    private void OnPlaybackerCompleted()
    {
        _countOfCompletedPlaybackers++;
        if (_countOfCompletedPlaybackers == _playbackers.Count)
            FinishPlaybacking();
    }

    public void FinishPlaybacking()
    {
        foreach (var paybackableComponent in _playbackers)
        {
            paybackableComponent.OnPlaybackCompleted -= OnPlaybackerCompleted;
        }

        if (_usedDefaultTicker) { _defaultTicker = null; _usedDefaultTicker = false; }

        OnPlaybackCompleted?.Invoke();
    }
}
