using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class Language_Button : MonoBehaviour
{
    [SerializeField] private LanguageConfig config;    
    [SerializeField] private Button button;

    private LocalizationManager localizationManager;

    [Inject]
    public void Construct(LocalizationManager _localizationManager)
    {
        localizationManager = _localizationManager;
    }

    private void OnEnable() => button.onClick.AddListener(SelectLanguage);
    private void OnDisable() => button.onClick.RemoveListener(SelectLanguage);

    private void SelectLanguage()
    {
        if (config != null)
        {
            localizationManager.ChangeLanguage(config.LocaleCode);
        }
    }
}