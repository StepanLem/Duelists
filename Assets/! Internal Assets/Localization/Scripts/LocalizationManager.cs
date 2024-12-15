using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
public class LocalizationManager
{
    private StringTable _activeTable;

    private const string TableName = "StringTable";

    public string GetField(string filedName)
    {
        return LocalizationSettings.StringDatabase.GetTable(TableName).GetEntry(filedName).GetLocalizedString();
    }

    public string GetSelectedLocale()
    {
        return LocalizationSettings.SelectedLocale.Formatter.ToString();
    }

    public void ChangeLanguage(string languageCode)
    {
        AsyncProcessor.StartRoutine(ChangeLanguageRoutine(languageCode));
    }

    private IEnumerator ChangeLanguageRoutine(string languageCode)
    {
        yield return LocalizationSettings.InitializationOperation;

        Locale newLocale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
        if (newLocale != null)
        {
            LocalizationSettings.SelectedLocale = newLocale;
        }
    }
}
