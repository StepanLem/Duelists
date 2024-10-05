using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackingSystem : MonoBehaviour
{
    [SerializeField] private List<PlaybackableEntity> _playbackableEntities;

    [SerializeField] private MonoTicker _defaultTicker;

    public bool IsPlaybacking { get; private set; }
    public event Action OnPlaybackingCompleted;

    private int _completedEntityPlaybacksCount;

    public void StartPlaybacking(GameRecord recordedGameData)
    {
        if (IsPlaybacking)
        {
            Debug.LogWarning("Проигрывание уже идёт");
            return;
        }

        IsPlaybacking = true;

        _completedEntityPlaybacksCount = 0;

        foreach (var playbackableEntity in _playbackableEntities)
        {
            playbackableEntity.OnPlaybackCompleted += OnEntityPlaybackCompleted;
            playbackableEntity.StartPlaybacking(recordedGameData, _defaultTicker);
        }
    }

    private void OnEntityPlaybackCompleted()
    {
        _completedEntityPlaybacksCount++;
        if (_completedEntityPlaybacksCount == _playbackableEntities.Count)
            FinishPlaybacking();
    }

    private void FinishPlaybacking()
    {
        foreach (var playbackableEntity in _playbackableEntities)
        {
            playbackableEntity.OnPlaybackCompleted -= FinishPlaybacking;
        }

        OnPlaybackingCompleted?.Invoke();

        IsPlaybacking = false;
        Debug.LogWarning("Проигрывание завершено");
    }
}
