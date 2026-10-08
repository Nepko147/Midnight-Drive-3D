using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager SingleOnScene { get; private set; }
    
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

    private void Awake()
    {
        if (SingleOnScene == null)
        {
            SingleOnScene = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;
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

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }
}
