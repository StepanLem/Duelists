﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IValueProvider : MonoBehaviour //временно пока не сделали пробрасывание в инспектор через интерфейсы.
                                                     //Потом сделать интерфейсом и убрать все override
{
    public abstract IValue GetValue();
    public abstract void SetValue(in IValue value);
}
