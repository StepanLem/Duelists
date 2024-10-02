using System;
using System.Collections.Generic;
using UnityEngine;

public class RecordedGameData
{
    public List<RecordedTargetData> RecordedTargetsByInstanceID = new();

    public float Duration;

    public void AddRecordedTargetDataByInstanceID(RecordedTargetData targetRecord, int instanceID)
    {
        //���� ����� ������ ������ ��:
        //"���� ����������� ��������� ��� target. ��� ���-�� �����������? ������ ������������, �� � ������ �������� ������ ��������� ������"
        RecordedTargetsByInstanceID.AddAtIndexWithExpand(targetRecord, instanceID);
    }

    public RecordedTargetData GetRecordedTargetDataByInstanceID(int instanceID)
    {
        return RecordedTargetsByInstanceID[instanceID];
    }
}

public class RecordedTargetData
{
    public List<RecordedValueData> RecordedValuesByInstanceID = new(); //�������� �� �������? � ��� ������ ���������� � ������� ����� c �������� ����������� ��������? � ��� ��� �� ������? �� ���� �������?

    internal void AddRecordedValueDataByInstanceID(RecordedValueData valueRecord, int instanceID)
    {
        //���� ����� ������ ������ ��:
        //"���� ����������� ��������� ��� target. ��� ���-�� �����������? ������ ������������, �� � ������ �������� ������ ��������� ������"
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
