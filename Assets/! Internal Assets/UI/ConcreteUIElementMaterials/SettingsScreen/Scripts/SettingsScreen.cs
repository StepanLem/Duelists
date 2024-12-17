using UnityEngine;
using Zenject;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] private SegmentedButton _languageButton;

    private LocalizationManager _localizationManager;

    [Inject]
    public void Cunstruct(LocalizationManager localizationManager)
    {
        _localizationManager = localizationManager;
    }

    private void Start()
    {
        _languageButton.SelectSegment(_localizationManager.GetSelectedLocale());
        _languageButton.OnSelectSegment += LanguageButton_SelectSegment;
    }

    private void LanguageButton_SelectSegment(string languageCode)
    {
        _localizationManager.ChangeLanguage(languageCode);
    }

    private void OnEnable()
    {
        _languageButton.SelectSegment(_localizationManager.GetSelectedLocale());
    }
}
