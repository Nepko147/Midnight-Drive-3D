using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour, ISaveable
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
        public float CurrentVolume { get; set; }
        public bool IsMuted { get; set; }
    }

    [System.Serializable]
    public class AudioSaveData
    {
        // Будем хранить настройки каждого канала в виде отдельного элемента списка
        public List<ChannelSaveEntry> channels = new();
    }

    [System.Serializable]
    public class ChannelSaveEntry
    {
        public string channelKey; // Сюда запишем уникальный prefsVolumeKey или prefsMuteKey
        public float volume;
        public bool isMuted;
    }

    [Header("Audio Setup")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioChannelSettings[] audioChannels;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    // Если одновременно-проигрываемых звуков дофига, то они могут начать глючить
    // Чтобы этого не произошло делаем Audio Pooling
    [Header("Pool Settings")]
    [Tooltip("Максимальное количество одновременно звучащих звуков. Не может быть меньше 1.")]
    [SerializeField, Min(SFXPOOLSIZE_MIN)] private int sfxPoolSize = 5; // Сколько звуков может звучать одновременно

    private const int SFXPOOLSIZE_MIN = 1;

    private List<AudioSource> sfxPool;
    private int nextPoolIndex = 0; // Счётчик для циклического переиспользования источников

    private const float MIXER_MIN = -30f;
    private const float MIXER_MAX = 20f;
    private const float MIXER_OFF = -80f;
    private const float MUTE_THRESHOLD = 0.01f;

    // Передаем ТИП канала, громкость и статус Mute
    public event Action<AudioChannelType, float, bool> OnAudioSettingsChanged;
    
    private const string AUDIO_SAVE_KEY = "audio_settings";

    public string SaveKey => AUDIO_SAVE_KEY;

    private void Awake()
    {
        if (SingleOnScene == null) //Защита от дубликатов
        {
            SingleOnScene = this;
        }
    }

    private void Start()
    {
        if (SingleOnScene == this) 
        {
            DontDestroyOnLoad(gameObject);

            InitializeSFXPool(); // Инициализируем пул вместо одного источника
            InitializeMusicSources(); // Инициализируем 1 источник (В текущем проекте пул не нужен)

            SaveManager.SingleOnScene.Register(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (SingleOnScene == this 
            && SaveManager.SingleOnScene != null)
        {
            SaveManager.SingleOnScene.Unregister(this);
        }
    }

    // Собираем данные для универсального сейва
    public string CaptureState()
    {
        AudioSaveData _dataToSave = new AudioSaveData();

        foreach (var _audiochannel in audioChannels)
        {
            _dataToSave.channels.Add(new ChannelSaveEntry
            {
                channelKey = _audiochannel.prefsVolumeKey,
                volume = _audiochannel.CurrentVolume,
                isMuted = _audiochannel.IsMuted
            });
        }

        return JsonUtility.ToJson(_dataToSave, true); // Превращаем данные в строку прямо здесь
    }

    // Восстанавливаем данные из универсального сейва
    public void RestoreState(string _stateJson)
    {
        AudioSaveData _savedData = JsonUtility.FromJson<AudioSaveData>(_stateJson);

        if (_savedData != null)
        {
            foreach (var _audioChannel in audioChannels)
            {
                var _savedChannel = _savedData.channels.Find(c => c.channelKey == _audioChannel.prefsVolumeKey);

                if (_savedChannel != null)
                {
                    _audioChannel.CurrentVolume = _savedChannel.volume;
                    _audioChannel.IsMuted = _savedChannel.isMuted;
                }
                else
                {
                    _audioChannel.CurrentVolume = AudioChannelSettings.DEFAULT_VALUE;
                    _audioChannel.IsMuted = false;
                }

                // Применяем настройки к микшеру сразу после загрузки
                ApplyState(_audioChannel);
            }
        }
    }

    public void SetDefaultState()
    {
        if (audioChannels != null && audioChannels.Length > 0)
        {
            foreach (var _audioChannel in audioChannels)
            {
                _audioChannel.CurrentVolume = AudioChannelSettings.DEFAULT_VALUE;
                _audioChannel.IsMuted = false;
                
                ApplyState(_audioChannel);
            }
        }
    }

    public AudioChannelSettings GetChannel(AudioChannelType _type)
    {
        return Array.Find(audioChannels, _c => _c.channelType == _type);
    }

    #region UI

    public void SetVolumeFromUI(AudioChannelType _type, float _newValue)
    {
        var _audioChannel = GetChannel(_type);

        if(_audioChannel != null)
        {
            _audioChannel.CurrentVolume = _newValue;
            _audioChannel.IsMuted = _audioChannel.CurrentVolume <= MUTE_THRESHOLD;
            
            ApplyState(_audioChannel);
        }
    }

    public void ToggleMuteFromUI(AudioChannelType _type)
    {
        var _audioChannel = GetChannel(_type);

        if (_audioChannel != null)
        {
            _audioChannel.IsMuted = !_audioChannel.IsMuted;

            if (!_audioChannel.IsMuted
                && _audioChannel.CurrentVolume <= MUTE_THRESHOLD)
            {
                _audioChannel.CurrentVolume = AudioChannelSettings.DEFAULT_VALUE;
            }
            
            ApplyState(_audioChannel);
            SaveManager.SingleOnScene.SaveGame();
        }
    }

    public void SaveAudioSettings()
    {
        if (SaveManager.SingleOnScene != null)
        {
            SaveManager.SingleOnScene.SaveGame();
            Debug.Log("[AudioManager] Настройки звука успешно сохранены!");
        }
    }

    private void ApplyState(AudioChannelSettings _audioChannel)
    {
        // 1. Обновляем микшер
        var _mixerVolume = _audioChannel.IsMuted ? MIXER_OFF : Mathf.Lerp(MIXER_MIN, MIXER_MAX, _audioChannel.CurrentVolume);
        audioMixer.SetFloat(_audioChannel.mixerParamName, _mixerVolume);

        // Оповещаем все подписанные UI элементы
        OnAudioSettingsChanged?.Invoke(_audioChannel.channelType, _audioChannel.CurrentVolume, _audioChannel.IsMuted);
    }

    #endregion

    #region Music

    private AudioSource music_activeSource;
    private AudioSource music_inactiveSource;
    private Coroutine music_fadeRoutine;

    private void InitializeMusicSources()
    {
        // Создаем два источника для плавного перекрестного затухания
        music_activeSource = gameObject.AddComponent<AudioSource>();
        music_activeSource.outputAudioMixerGroup = musicGroup;

        music_inactiveSource = gameObject.AddComponent<AudioSource>();
        music_inactiveSource.outputAudioMixerGroup = musicGroup;
    }

    public void Music_Play(MusicAsset _musicAsset, float _fadeDuration = 1.5f)
    {
        if (_musicAsset != null
            && music_activeSource.clip != _musicAsset.Clip)
        {
            if (music_fadeRoutine != null)
            {
                StopCoroutine(music_fadeRoutine);
            }

            music_fadeRoutine = StartCoroutine(Music_CrossfadeRoutine(_musicAsset, _fadeDuration));
        }
    }

    private IEnumerator Music_CrossfadeRoutine(MusicAsset _musicAsset, float _duration)
    {
        // Готовим второй источник
        music_inactiveSource.clip = _musicAsset.Clip;
        music_inactiveSource.loop = _musicAsset.Loop;
        music_inactiveSource.volume = 0;
        music_inactiveSource.Play();

        float _elapsed = 0f;
        float _targetVolume = _musicAsset.Volume;
        float _startVolume = music_activeSource.volume;

        while (_elapsed < _duration)
        {
            _elapsed += Time.unscaledDeltaTime;
            float _t = _elapsed / _duration;

            // Один трек становится тише, второй громче
            music_activeSource.volume = Mathf.Lerp(_startVolume, 0f, _t);
            music_inactiveSource.volume = Mathf.Lerp(0f, _targetVolume, _t);
            yield return null;
        }

        music_activeSource.Stop();

        // Меняем источники местами
        var _temp = music_activeSource;
        music_activeSource = music_inactiveSource;
        music_inactiveSource = _temp;
    }

    public void Music_Pause() => music_activeSource.Pause();
    
    public void Music_UnPause() => music_activeSource.UnPause();

    #endregion

    #region SFX

    private void InitializeSFXPool()
    {
        sfxPool = new List<AudioSource>();

        if (sfxPoolSize < SFXPOOLSIZE_MIN)
        {
            Debug.LogError($"[AudioManager] CRITICAL ERROR: sfxPoolSize is less than {SFXPOOLSIZE_MIN}! Pool size fallback to the minimum allowed value: {SFXPOOLSIZE_MIN}");
            sfxPoolSize = SFXPOOLSIZE_MIN;
        }

        for (int _i = 0; _i < sfxPoolSize; _i++)
        {
            // Создаем дочерний объект для каждого источника в пуле
            GameObject _poolObject = new GameObject($"SFX_Source_{_i}");
            _poolObject.transform.SetParent(transform);

            AudioSource _source = _poolObject.AddComponent<AudioSource>();
            _source.outputAudioMixerGroup = sfxGroup;
            _source.playOnAwake = false;

            sfxPool.Add(_source);
        }
    }

    private AudioSource GetAvailableSFXSource()
    {
        // Берём источник по круговому индексу.
        AudioSource _source = sfxPool[nextPoolIndex];

        // Сдвигаем индекс на следующий элемент (если дошли до конца — сбрасываем в 0)
        nextPoolIndex = (nextPoolIndex + 1) % sfxPoolSize; // Метод "Кольцевой буфер"

        return _source;
    }

    public void SFX_Play(SFXAsset _audioXAsset)
    {
        if (_audioXAsset == null)
        {
            Debug.LogWarning($"AudioEvent не найден!");
            return;
        }

        // Берем свободный источник из пула
        AudioSource _freeSource = GetAvailableSFXSource();

        // ScriptableObject (AudioEvent) проигрывает сам себя через sfxSource менеджера
        _audioXAsset.Play(_freeSource);
    }

    #endregion
}
