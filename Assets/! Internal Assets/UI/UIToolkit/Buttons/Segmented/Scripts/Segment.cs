using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Segment : MonoBehaviour
{
    [SerializeField] private string _value;
    [Space]
    [SerializeField] private TextMeshProUGUI _labelText;

    private Image _container;
    private Button _button;

    private SegmentedButton _segmentedButton;

    public string Value => _value;

    private void Awake()
    {
        _container = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        if (transform.parent.TryGetComponent(out SegmentedButton segmentedButton))
        {
            _segmentedButton = segmentedButton;
        }
        else
        {
            Debug.LogError($"SEGMENT NOT HAVE PARENT WITH {nameof(SegmentedButton)} SCRIPT");
        }

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => _segmentedButton.OnSelectSegment(_value));

        _segmentedButton.SelectSegment += SegmentedButton_SelectSegment;
    }

    private void SegmentedButton_SelectSegment(string segmentValue)
    {
        if (segmentValue == _value)
        {
            SetPressedState();
        }
        else
        {
            SetEnabledState();
        }
    }

    private void SetPressedState()
    {
        SetState(false, _segmentedButton.PressedColors);
    }

    private void SetEnabledState()
    {
        SetState(true, _segmentedButton.EnabledColors);
    }

    private void SetState(bool interactable, (Color, Color) colors)
    {
        _button.interactable = interactable;
        _container.color = colors.Item1;
        _labelText.color = colors.Item2;
    }
}
