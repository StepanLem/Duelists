using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct SnapshotOfValue
{
    public SnapshotOfValue(IValue value, float timeOfCreatingFromRecordStart)
    {
        Value = value;
        TimeOfCreatingFromRecordStart = timeOfCreatingFromRecordStart;
    }

    public readonly IValue Value;
    public readonly float TimeOfCreatingFromRecordStart;
}
