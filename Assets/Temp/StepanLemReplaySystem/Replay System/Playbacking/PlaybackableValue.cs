using System;
using UnityEngine;

public class PlaybackableValue : MonoBehaviour
{
    [Tooltip("Передаёт значение для записи в него данных из RecordedData по ValueDataID.")]
    [SerializeField] private IValueProvider _valueProvider;

    [Tooltip("Нужен для привязки записи к объекту во время его сериализации/десериализации.")]
    public int ValueDataID;

    [Tooltip("If null, use defaultTicker")]
    [SerializeField] private MonoTicker _ticker;
    private bool _usedDefaultTicker;

    public event Action OnPlaybackCompleted;

    private RecordedValueData _currentPlaybackingValueData;
    private int _maxIndexInRecord;
    private int _currentIndexInRecord;

    public void StartPlaybacking(RecordedTargetData recordedTargetData, MonoTicker defaultTicker)
    {
        _currentPlaybackingValueData = recordedTargetData.GetRecordedValueDataByInstanceID(ValueDataID);

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

        _currentPlaybackingValueData = null;
        if (_usedDefaultTicker) { _ticker = null; _usedDefaultTicker = false; }

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
