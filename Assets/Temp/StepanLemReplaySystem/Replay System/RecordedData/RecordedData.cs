using System.Collections.Generic;

public class GameRecord
{
    public List<EntityRecord> EntityRecordsByEntityID = new();

    public float Duration;

    public void AddEntityRecordByID(EntityRecord record, int entityID)
    {
        //Если здесь высрет ошибку то:
        //"Было наслаивание инстансов для target. Они где-то повторяются? Работа продолжилась, но в ячейке остались только последние данные"
        EntityRecordsByEntityID.AddAtIndexWithExpand(record, entityID);
    }

    public EntityRecord GetEntityRecordByID(int entityID)
    {
        return EntityRecordsByEntityID[entityID];
    }
}

public class EntityRecord
{
    public List<IDataRecord> DataRecordsByDataID = new(); //поменять на словарь? а как делают сохранения в больших играх c тысячами сохраняемых объектов? В чём они всё хранят? По чему находят?

    public void AddDataRecordByID(IDataRecord record, int dataID)
    {
        //Если здесь высрет ошибку то:
        //"Было наслаивание инстансов для target. Они где-то повторяются? Работа продолжилась, но в ячейке остались только последние данные"
        DataRecordsByDataID.AddAtIndexWithExpand(record, dataID);
    }

    public IDataRecord GetDataRecorByID(int instanceID)
    {
        return DataRecordsByDataID[instanceID];
    }
}

public interface IDataRecord
{
    
}

public class DataRecord<T> : IDataRecord
{
    public List<Snapshots<T>> Snapshots = new();
}
