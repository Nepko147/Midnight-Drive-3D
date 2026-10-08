using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class DynamicTextExample : MonoBehaviour
{
    [SerializeField] private TMP_Text uiText;
    [SerializeField] private LocalizedString localizedString;

    private void OnEnable()
    {
        // ѕодписываемс€ на событие смены €зыка, чтобы текст обновл€лс€ автоматически
        localizedString.StringChanged += UpdateScoreText;
    }

    private void OnDisable()
    {
        localizedString.StringChanged -= UpdateScoreText;
    }

    private void UpdateScoreText(string _localizedValue)
    {
        uiText.text = _localizedValue;
    }
}
