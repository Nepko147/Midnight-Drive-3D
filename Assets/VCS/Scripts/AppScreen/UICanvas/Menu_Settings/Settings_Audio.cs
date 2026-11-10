using UnityEngine;

public class Settings_Audio : MonoBehaviour
{
    [Header("Channel Settings")]
    [SerializeField] private AudioChannelType audioChannelType; // Указываем тип в инспекторе (Master, Music или SFX)

    [Header("UI Links")]
    [SerializeField] private Audio_Button volumeButton;
    [SerializeField] private Audio_Slider volumeSlider;
    
    private AudioManager cachedAudioManager; // Кэшированная ссылка AudioManager
    // В Unity при уничтожении сцены синглтон может очиститься раньше,
    // чем вызовется OnDisable обычного UI-элемента.
    // Из-за этого отписка (AudioManager.SingleOnScene -= ...) может не сработать.
    // Из-за чего, в свою очередь возможна утечка памяти.
    // Решение: Кэшировать ссылку на менеджер при подписке.

    private bool isSubscribed = false;

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
            volumeSlider.OnDragEnded += OnSliderDragEnded;
        }

        // Подписаться на менеджер
        TrySubscribeToManager();
    }

    private void Start()
    {
        // Гарантированная подписка при запуске
        TrySubscribeToManager();
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
            volumeSlider.OnDragEnded -= OnSliderDragEnded;
        } 

        if (cachedAudioManager != null)
        {
            cachedAudioManager.OnAudioSettingsChanged -= OnManagerAudioSettingsChanged;
            cachedAudioManager = null; // Чистим кеш
        }

        isSubscribed = false;
    }

    private void TrySubscribeToManager()
    {
        // Подписывыемся на менеджер, только если он существует и если еще не подписались
        if (AudioManager.SingleOnScene != null
            && cachedAudioManager == null
            && !isSubscribed)
        {
            cachedAudioManager = AudioManager.SingleOnScene;
            cachedAudioManager.OnAudioSettingsChanged += OnManagerAudioSettingsChanged;
            isSubscribed = true;

            var _audioChannel = AudioManager.SingleOnScene.GetChannel(audioChannelType);

            if (_audioChannel != null)
            {
                // Синхронизируем UI с актуальными данными из менеджера
                UpdateUI(_audioChannel.CurrentVolume, _audioChannel.IsMuted);
            }
        } 
    }

    private void OnSliderChanged(float _value) => cachedAudioManager?.SetVolumeFromUI(audioChannelType, _value);
    
    private void OnMuteClicked() => cachedAudioManager?.ToggleMuteFromUI(audioChannelType);

    private void OnSliderDragEnded() => cachedAudioManager?.SaveAudioSettings();

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
