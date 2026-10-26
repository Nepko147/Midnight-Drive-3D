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
    // Из-за этого возможны утечки памяти.
    // Решение: Кэшировать ссылку на менеджер при подписке.

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
        // Гарантированный подписка при запуске
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

        if (cachedAudioManager != null)
        {
            cachedAudioManager.OnAudioSettingsChanged -= OnManagerAudioSettingsChanged;
            cachedAudioManager = null; // Чистим кеш
        }
    }

    private void SubscribeToManager()
    {
        // Подписывыемся на менеджер, только если он существует и если еще не подписались
        if (AudioManager.SingleOnScene != null
            && cachedAudioManager == null)
        {
            cachedAudioManager = AudioManager.SingleOnScene;
            cachedAudioManager.OnAudioSettingsChanged += OnManagerAudioSettingsChanged;

            var _audioChannel = AudioManager.SingleOnScene.GetChannel(audioChannelType);

            if (_audioChannel != null)
            {
                // Синхронизируем UI с актуальными данными из менеджера
                UpdateUI(_audioChannel.CurrentVolume, _audioChannel.IsMuted);
            }
        } 
    }

    // Лямбды:
    private void OnSliderChanged(float _value) => cachedAudioManager?.SetVolumeFromUI(audioChannelType, _value);
    private void OnMuteClicked() => cachedAudioManager?.ToggleMuteFromUI(audioChannelType);

    // То же самое, но по классике:
    //private void OnSliderChanged(float _value)
    //{
    //    if (cachedAudioManager != null)
    //    {
    //        cachedAudioManager.SetVolumeFromUI(audioChannelType, _value);
    //    }
    //}
    //
    //private void OnMuteClicked()
    //{
    //    if (cachedAudioManager != null)
    //    {
    //        cachedAudioManager.ToggleMuteFromUI(audioChannelType);
    //    }   
    //}

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
