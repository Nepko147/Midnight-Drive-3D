using UnityEngine;

public class Settings_Audio : MonoBehaviour
{
    [Header("Channel Settings")]
    [SerializeField] private AudioChannelType audioChannelType; // Указываем тип в инспекторе (Master, Music или SFX)

    [Header("UI Links")]
    [SerializeField] private Audio_Button volumeButton;
    [SerializeField] private Audio_Slider volumeSlider;

    private bool isSubscribedToManager = false;

    private void OnEnable()
    {
        // Подписываемся на события UI
        if (volumeButton != null) 
        { 
            volumeButton.OnClicked += OnMuteClicked;
        } 

        if (volumeSlider != null) 
        { 
            volumeSlider.OnValueChanged += OnSliderChanged;
        }

        // Подписаться на менеджер
        SubscribeToManager();
    }

    private void Start()
    {
        // Гарантированный подписку при запуске
        SubscribeToManager();
    }

    private void OnDisable()
    {
        // Отписываемся, чтобы избежать утечек памяти!
        if (volumeButton != null) 
        {
            volumeButton.OnClicked -= OnMuteClicked;
        }

        if (volumeSlider != null) 
        {
            volumeSlider.OnValueChanged -= OnSliderChanged;
        } 

        if (AudioManager.SingleOnScene != null)
        {
            AudioManager.SingleOnScene.OnAudioSettingsChanged -= OnManagerAudioSettingsChanged;
        }
    }

    private void SubscribeToManager()
    {
        // Если уже подписаны или менеджера еще нет в принципе — ничего не делаем
        if (isSubscribedToManager 
            || AudioManager.SingleOnScene == null)
        {
            return;
        }            

        AudioManager.SingleOnScene.OnAudioSettingsChanged += OnManagerAudioSettingsChanged;
        isSubscribedToManager = true;

        var _audioChannel = AudioManager.SingleOnScene.GetChannel(audioChannelType);

        if (_audioChannel != null)
        {
            // Синхронизируем UI с актуальными данными из менеджера
            UpdateUI(_audioChannel.currentVolume, _audioChannel.isMuted);
        }
    }

    private void OnSliderChanged(float _value)
    {
        if (AudioManager.SingleOnScene != null)
        {
            AudioManager.SingleOnScene.SetVolumeFromUI(audioChannelType, _value);
        }
    }

    private void OnMuteClicked()
    {
        if (AudioManager.SingleOnScene != null)
        {
            AudioManager.SingleOnScene.ToggleMuteFromUI(audioChannelType);
        }   
    }

    private void OnManagerAudioSettingsChanged(AudioChannelType _targetAudioChannel, float _volume, bool _isMuted)
    {
        if (_targetAudioChannel == audioChannelType)
        {
            UpdateUI(_volume, _isMuted);
        }
    }

    private void UpdateUI(float _volume, bool _isMuted)
    {
        if (volumeButton != null)
        {
            if (_isMuted)
            {
                volumeButton.SetMuteVisual(_isMuted);
            }
            else
            {
                volumeButton.Value = _volume;
            }
        }

        if (volumeSlider != null)
        {
            volumeSlider.Value = _isMuted ? 0f : _volume;
        }
    }
}
