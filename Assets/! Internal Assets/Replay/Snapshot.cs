using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public readonly struct Snapshot<T>
{
    public readonly T Value;
    public readonly DateTime TimeStamp;
    public Snapshot(T value, DateTime timeStamp)
    {
        Value = value;
        TimeStamp = timeStamp;
    }
}
