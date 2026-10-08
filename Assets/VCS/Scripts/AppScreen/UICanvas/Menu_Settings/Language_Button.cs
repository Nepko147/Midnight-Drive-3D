using UnityEngine;
using UnityEngine.UI;

public class Language_Button : MonoBehaviour
{
    [SerializeField] private LanguageConfig config;
    [SerializeField] private LocalizationManager localizationManager;
    [SerializeField] private Button button;

    private void OnEnable() => button.onClick.AddListener(SelectLanguage);
    private void OnDisable() => button.onClick.RemoveListener(SelectLanguage);

    private void SelectLanguage()
    {
        if (config != null)
        {
            localizationManager.ChangeLanguage(config.LocaleId);
        }
    }
}