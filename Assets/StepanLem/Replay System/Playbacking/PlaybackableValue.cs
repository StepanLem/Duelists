using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackableValue : MonoBehaviour
{
    [SerializeField] private IValueProvider _valueProvider;

    [SerializeField] private MonoTicker _ticker;

    private RecordedValueData _currentPlaybackingValueData;
    private int _maxIndexInRecord;
    private int _currentIndexInRecord;

    public event Action OnPlaybackCompleted;

    /// <summary>
    /// ID нужен для привязки записи к объекту во время его сериализации/десериализации.
    /// </summary>
    public int InstanceID;

    public void StartPlaybacking(RecordedTargetData recordedTargetData, MonoTicker defaultTicker)
    {
        _currentPlaybackingValueData = recordedTargetData.GetRecordedValueDataByInstanceID(InstanceID);

        if(_currentPlaybackingValueData == null)
        {
            //Debug.Log("В RecordedData нет записи о объекте");
            OnPlaybackCompleted?.Invoke();
            return;
        }

        if (_ticker == null) _ticker = defaultTicker;

        _maxIndexInRecord = _currentPlaybackingValueData.Snapshots.Count - 1;
        _currentIndexInRecord = 0;

        _ticker.Fire += SetSnupshotValue;
    }

    public void FinishPlaybacking()
    {
        _ticker.Fire -= SetSnupshotValue;

        OnPlaybackCompleted?.Invoke();
    }

    public void SetSnupshotValue()
    {
        if (_currentIndexInRecord >= _maxIndexInRecord)
        {
            FinishPlaybacking();
            return;
        }

        _valueProvider.SetValue(_currentPlaybackingValueData.Snapshots[_currentIndexInRecord + 1].Value);//TODO: передавать по ссылке для оптимизации?
                                                                                                         //TODO: SetValueWithIntropolation()
        _currentIndexInRecord++;
    }
}
