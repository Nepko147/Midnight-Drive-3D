using UnityEngine;

[CreateAssetMenu(fileName = "NewLanguageConfig", menuName = "Localization/Language Config")]
public class LanguageConfig : ScriptableObject
{
    [SerializeField] private string languageName;
    [SerializeField] private string localeCode; // Используем ISO-код (например, "en", "ru")

    public string LocaleCode => localeCode;
    public string LanguageName => languageName;
}