using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    private bool isChangingLanguage = false;

    public void ChangeLanguage(int _localeId)
    {
        if (isChangingLanguage)
        {
            return;
        }

        StartCoroutine(SetLocaleCoroutine(_localeId));
    }

    private IEnumerator SetLocaleCoroutine(int _localeId)
    {
        isChangingLanguage = true;
                
        yield return LocalizationSettings.InitializationOperation;

        if (_localeId >= 0 
            && _localeId < LocalizationSettings.AvailableLocales.Locales.Count)
        {
            var _targetLocale = LocalizationSettings.AvailableLocales.Locales[_localeId];
            LocalizationSettings.SelectedLocale = _targetLocale;
        }

        isChangingLanguage = false;
    }
}