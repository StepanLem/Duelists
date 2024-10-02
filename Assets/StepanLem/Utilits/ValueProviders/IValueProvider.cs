using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IValueProvider: MonoBehaviour //�������� ���� �� ������� ������������� � ��������� ����� ����������.
                                                    //����� ������� ����������� � ������ ��� override
{
    public abstract IValue GetValue();
    public abstract void SetValue(IValue value);
}
