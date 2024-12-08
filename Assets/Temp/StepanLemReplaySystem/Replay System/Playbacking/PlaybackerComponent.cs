using System;
using UnityEngine;

public sealed class PlaybackerComponent : MonoBehaviour
{
    [Tooltip("Для записи значения фабрика должена создавать именно RecordReaderWithSetter<T>")]
    [SerializeField, InterfaceType(typeof(IReaderFactory))]
    private UnityEngine.Object _factoryOfReaderWithSetter;
    private IReaderFactory ReaderFactory => _factoryOfReaderWithSetter as IReaderFactory;

    [Tooltip("Нужен для привязки записи к объекту во время его сериализации/десериализации.")]
    public int ValueDataID;

    [Tooltip("If null, use defaultTicker")]
    [SerializeField] private MonoTicker _ticker;
    private bool _usedDefaultTicker;

    public event Action OnPlaybackCompleted;

    public IRecordReader Reader;

    public void StartPlaybacking(EntityRecord targetRecord, MonoTicker defaultTicker)
    {
        Reader = ReaderFactory.CreateReader();

        var dataRecord = targetRecord.GetDataRecorByID(ValueDataID);
        Reader.SetRecord(dataRecord);

        Reader.OnRecordEnd += FinishPlaybacking;

        if (_ticker == null) { _ticker = defaultTicker; _usedDefaultTicker = true; }
        _ticker.Fire += Reader.ReadNextSnapshot;
    }

    public void FinishPlaybacking()//мб переименовать на On...
    {
        Reader.OnRecordEnd -= FinishPlaybacking;
        _ticker.Fire -= Reader.ReadNextSnapshot;

        if (_usedDefaultTicker) { _ticker = null; _usedDefaultTicker = false; }
        Reader = null;

        OnPlaybackCompleted?.Invoke();
    }
}

public interface IRecordReader
{
    public void SetRecord(IDataRecord recordToRead);
    public void ReadNextSnapshot();
    public Action OnRecordEnd { get; set; }
}

public class RecordReaderWithSetter<T> : IRecordReader
{
    private Action<T> _valueSetter;
    private DataRecord<T> _recordToRead;

    private int _maxIndexInRecord;
    private int _currentIndexInRecord;

    public Action OnRecordEnd { get; set; }

    public RecordReaderWithSetter(Action<T> valueSetter)
    {
        /*if (valueRecord == null)
        {
            //Debug.Log("Нет записи о объекте");
            OnRecordEnd?.Invoke();
            return;
        }*/

        _valueSetter = valueSetter;
        _currentIndexInRecord = 0;//в будущем можно включать в плэйбэк элементы не только на начале записи
    }

    public void SetRecord(IDataRecord recordToRead)
    {
        _recordToRead = (DataRecord<T>)recordToRead;
        _maxIndexInRecord = _recordToRead.Snapshots.Count - 1;
    }

    public void ReadNextSnapshot()
    {
        if (_currentIndexInRecord >= _maxIndexInRecord)
        {
            OnRecordEnd?.Invoke();
            return;
        }
        _currentIndexInRecord++;

        _valueSetter(_recordToRead.Snapshots[_currentIndexInRecord].Value);//TODO: передавать по ссылке для оптимизации?
        //TODO: SetValueWithIntropolation(_currentIndex - 1, _currentIndex)
    }
}
