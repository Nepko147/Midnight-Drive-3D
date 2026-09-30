using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewLanguageConfig", menuName = "Localization/Language Config")]

public class LanguageConfig : ScriptableObject
{
    [SerializeField] private string languageName; // Например, "English"
    [SerializeField] private Locale locale;       // Ссылка на конкретную локаль из Unity Localization
    [SerializeField] private int localeId;

    public int LocaleId => localeId;
    public string LanguageName => languageName;
}