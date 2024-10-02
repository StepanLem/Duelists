using System;
using System.Collections.Generic;
using UnityEngine;

public class RecordedGameData
{
    public List<RecordedTargetData> RecordedTargetsByInstanceID = new();

    public float Duration;

    public void AddRecordedTargetDataByInstanceID(RecordedTargetData targetRecord, int instanceID)
    {
        //Если здесь высрет ошибку то:
        //"Было наслаивание инстансов для target. Они где-то повторяются? Работа продолжилась, но в ячейке остались только последние данные"
        RecordedTargetsByInstanceID.AddAtIndexWithExpand(targetRecord, instanceID);
    }

    public RecordedTargetData GetRecordedTargetDataByInstanceID(int instanceID)
    {
        return RecordedTargetsByInstanceID[instanceID];
    }
}

public class RecordedTargetData
{
    public List<RecordedValueData> RecordedValuesByInstanceID = new(); //поменять на словарь? а как делают сохранения в больших играх c тысячами сохраняемых объектов? В чём они всё хранят? По чему находят?

    internal void AddRecordedValueDataByInstanceID(RecordedValueData valueRecord, int instanceID)
    {
        //Если здесь высрет ошибку то:
        //"Было наслаивание инстансов для target. Они где-то повторяются? Работа продолжилась, но в ячейке остались только последние данные"
        RecordedValuesByInstanceID.AddAtIndexWithExpand(valueRecord, instanceID);
    }

    public RecordedValueData GetRecordedValueDataByInstanceID(int instanceID)
    {
        return RecordedValuesByInstanceID[instanceID];
    }
}

public class RecordedValueData
{
    public List<SnapshotOfValue> Snapshots = new();
}
