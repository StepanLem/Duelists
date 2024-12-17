using UnityEngine;
using Zenject;

public class SetLanguageSegmentedButton : MonoBehaviour
{
    private SegmentedButton _button;

    private LocalizationManager _localizationManager;

    [Inject]
    public void Cunstruct(LocalizationManager localizationManager)
    {
        _localizationManager = localizationManager;
    }

    private void Awake()
    {
        _button = GetComponent<SegmentedButton>();
    }

    private void Start()
    {
        _button.SelectSegment(_localizationManager.GetSelectedLocale());
        _button.OnSelectSegment += LanguageButton_SelectSegment;
    }

    private void LanguageButton_SelectSegment(string languageCode)
    {
        _localizationManager.ChangeLanguage(languageCode);
    }

    private void OnEnable()
    {
        _button.SelectSegment(_localizationManager.GetSelectedLocale());
    }
}
