using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    private bool isChangingLanguage = false;

    public void ChangeLanguage(string _localeCode)
    {
        if (isChangingLanguage)
        {
            return;
        }

        StartCoroutine(SetLocaleCoroutine(_localeCode));
    }

    private IEnumerator SetLocaleCoroutine(string _localeCode)
    {
        isChangingLanguage = true;
                
        yield return LocalizationSettings.InitializationOperation;

        var _targetLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(_localeCode));

        if(_targetLocale != null)
        {
            LocalizationSettings.SelectedLocale = _targetLocale;
        }
        else
        {
            Debug.LogError($"[LocalizationManager] Локаль с кодом {_localeCode} не найдена!");
        }

        isChangingLanguage = false;
    }
}