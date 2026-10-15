using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager SingleOnScene { get; private set; }

    [System.Serializable]
    public class AudioChannelSettings
    {
        public AudioChannelType channelType;
        public string mixerParamName;
        public string prefsVolumeKey;
        public string prefsMuteKey;

        [HideInInspector] public const float DEFAULT_VALUE = 0.6f;
        [HideInInspector] public float currentVolume;
        [HideInInspector] public bool isMuted;
    }

    [Header("Audio Setup")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioChannelSettings[] audioChannels;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Audio Tracks")]
    [SerializeField] private Sound[] musicSounds;
    [SerializeField] private Sound[] sfxSounds;

    private AudioSource musicSource;    

    private AudioSource sfxSource;
    private const float SFXSOURCE_PITCH_MIN = 0.95f;
    private const float SFXSOURCE_PITCH_MAX = 1.05f;

    private const float MIXER_MIN = -30f;
    private const float MIXER_MAX = 20f;
    private const float MIXER_OFF = -80f;
    private const float MUTE_THRESHOLD = 0.01f;

    // Передаем ТИП канала, громкость и статус Mute
    public event Action<AudioChannelType, float, bool> OnAudioSettingsChanged;

    private void Awake()
    {
        if (SingleOnScene == null)
        {
            SingleOnScene = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        foreach (var _audioChannel in audioChannels)
        {
            ApplyState(_audioChannel);
        }
    }

    private void InitializeAudioSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;
    }

    public AudioChannelSettings GetChannel(AudioChannelType _type)
    {
        return Array.Find(audioChannels, _c => _c.channelType == _type);
    }

    public void SetVolumeFromUI(AudioChannelType _type, float _newValue)
    {
        var _audioChannel = GetChannel(_type);

        if(_audioChannel != null)
        {
            _audioChannel.currentVolume = _newValue;
            _audioChannel.isMuted = _audioChannel.currentVolume <= MUTE_THRESHOLD;

            SaveSettings(_audioChannel);
            ApplyState(_audioChannel);
        }
    }

    public void ToggleMuteFromUI(AudioChannelType _type)
    {
        var _audioChannel = GetChannel(_type);

        if (_audioChannel != null)
        {
            _audioChannel.isMuted = !_audioChannel.isMuted;

            if (!_audioChannel.isMuted
                && _audioChannel.currentVolume <= MUTE_THRESHOLD)
            {
                _audioChannel.currentVolume = AudioChannelSettings.DEFAULT_VALUE;
            }

            SaveSettings(_audioChannel);
            ApplyState(_audioChannel);
        }
    }

    private void ApplyState(AudioChannelSettings _audioChannel)
    {
        // 1. Обновляем микшер
        var _mixerVolume = _audioChannel.isMuted ? MIXER_OFF : Mathf.Lerp(MIXER_MIN, MIXER_MAX, _audioChannel.currentVolume);
        audioMixer.SetFloat(_audioChannel.mixerParamName, _mixerVolume);

        // Оповещаем все подписанные UI элементы
        OnAudioSettingsChanged?.Invoke(_audioChannel.channelType, _audioChannel.currentVolume, _audioChannel.isMuted);
    }

    private void LoadSettings()
    {
        foreach (var _audioChannel in audioChannels)
        {
            _audioChannel.currentVolume = PlayerPrefs.GetFloat(_audioChannel.prefsVolumeKey, AudioChannelSettings.DEFAULT_VALUE);
            _audioChannel.isMuted = PlayerPrefs.GetInt(_audioChannel.prefsMuteKey, 1) == 0;
        }
    }

    private void SaveSettings(AudioChannelSettings _audioChannel)
    {
        if (!_audioChannel.isMuted)
        {
            PlayerPrefs.SetFloat(_audioChannel.prefsVolumeKey, _audioChannel.currentVolume);
        }

        PlayerPrefs.SetInt(_audioChannel.prefsMuteKey, _audioChannel.isMuted ? 0 : 1);
        PlayerPrefs.Save();
    }

    public void PlayMusic(string _name)
    {
        var _sound = Array.Find(musicSounds, _s => _s.name == _name);

        if (_sound == null)
        {
            Debug.LogWarning($"Музыка: {_name} не найдена!");
            return;
        }

        musicSource.clip = _sound.clip;
        musicSource.Play();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }

    public void PlaySFX(string _name)
    {
        var _sound = Array.Find(sfxSounds, _s => _s.name == _name);

        if (_sound == null)
        {
            Debug.LogWarning($"Звук: {_name} не найден!");
            return;
        }

        sfxSource.pitch = UnityEngine.Random.Range(SFXSOURCE_PITCH_MIN, SFXSOURCE_PITCH_MAX);
        sfxSource.PlayOneShot(_sound.clip);
    }

    [System.Serializable] // Попробовать перенести/превратить в ScriptableObject
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }
}
