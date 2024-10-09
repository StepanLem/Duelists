using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private CardOption[] _options;
    [SerializeField] private int _initialOptionIndex = 0;
    [Space]
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private TextMeshProUGUI _optionText;
    [SerializeField] private Image _optionIcon;

    private int _selectedOptionIndex;

    public string Value { get; private set; }

    public void ChangeHeader(string header)
    {
        _header.text = header;
    }

    public void SetOption(string optionValue)
    {
        for (int i = 0; i < _options.Length; i++)
        {
            if (_options[i].Value == optionValue)
            {
                SetOption(_options[i]);
            }
        }
    }

    public void SetPreviousOption()
    {
        _selectedOptionIndex = Math.Max(_selectedOptionIndex - 1, 0);
        SetOption(_options[_selectedOptionIndex]);
    }

    public void SetNextOption()
    {
        _selectedOptionIndex = Math.Min(_selectedOptionIndex + 1, _options.Length - 1);
        SetOption(_options[_selectedOptionIndex]);
    }

    private void Awake()
    {
        _selectedOptionIndex = _initialOptionIndex;
        SetOption(_options[_selectedOptionIndex]);
    }

    private void SetOption(CardOption option)
    {
        Value = option.Value;

        _optionText.text = option.Text;
        _optionIcon.sprite = option.Icon;
    }
}
