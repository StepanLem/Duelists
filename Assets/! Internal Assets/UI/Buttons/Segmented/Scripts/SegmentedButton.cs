using System;
using UnityEngine;

public class SegmentedButton : MonoBehaviour
{
    [SerializeField] private int _selectedSegmentIndex = 0;
    [Space]
    [SerializeField] private Color _enabledContainerColor;
    [SerializeField] private Color _enabledLabelTextColor;
    [Space]
    [SerializeField] private Color _pressedContainerColor;
    [SerializeField] private Color _pressedLabelTextColor;

    public (Color, Color) EnabledColors => (_enabledContainerColor, _enabledLabelTextColor);
    public (Color, Color) PressedColors => (_pressedContainerColor, _pressedLabelTextColor);

    public string Value { get; private set; }

    public event Action<string> SelectSegment;

    public void OnSelectSegment(string segmentValue)
    {
        Value = segmentValue;
        SelectSegment?.Invoke(segmentValue);

        Debug.Log(Value);
    }

    private void Awake()
    {
        OnSelectSegment(GetComponentsInChildren<Segment>()[_selectedSegmentIndex].Value);
    }
}
