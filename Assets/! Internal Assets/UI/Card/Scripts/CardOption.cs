using System;
using UnityEngine;

[Serializable]
public class CardOption
{
    [SerializeField] private string _value;
    [Space]
    [SerializeField] private string _text;
    [SerializeField] private Sprite _icon;

    public string Value => _value;

    public string Text => _text;
    public Sprite Icon => _icon;
}