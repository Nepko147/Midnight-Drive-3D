using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Audio_Button : MonoBehaviour
{
    [Header("Audio Setup")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string audioMixer_exposedParamName; // Имя парамметра из микшера
    [SerializeField] private string PlayerPreferenceKey; // Имя параметра для сохранения в PlayerPrefs

    [Header("UI Elements")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    private const float VOLUME_MUTE = -80f;  // -80 dB - полная тишина
    private const float VOLUME_DEFAULT = 0f; // 0 dB - исходная громкость трека

    private bool isMuted = false;

    private void Start()
    {
        // Загружаем сохраненное состояние (0 = звук есть, 1 = выключен)
        isMuted = PlayerPrefs.GetInt(PlayerPreferenceKey, 0) == 1;

        ApplyMuteState();
    }

    private void ApplyMuteState()
    {
        if (isMuted)
        {            
            audioMixer.SetFloat(audioMixer_exposedParamName, VOLUME_MUTE);
            buttonImage.sprite = soundOffSprite;
        }
        else
        {            
            audioMixer.SetFloat(audioMixer_exposedParamName, VOLUME_DEFAULT);
            buttonImage.sprite = soundOnSprite;
        }
    }

    public void OnClick()
    {
        isMuted = !isMuted;

        // Сохраняем состояние, чтобы настройки не слетали при перезапуске
        PlayerPrefs.SetInt(PlayerPreferenceKey, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMuteState();
    }
}
