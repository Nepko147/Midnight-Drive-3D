using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewSFXAsset", menuName = "Audio/SFX Asset")]
public class SFXAsset : ScriptableObject
{
    [SerializeField] private AudioClip[] clips;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    // «адаЄм динамику. ƒаже если у нас всего один звук, он всЄ рано
    // будет звучать по разному, при каждом воспроизведении
    [Range(0.1f, 2f)]
    [SerializeField] private float pitchMin = 0.95f;
    [Range(0.1f, 2f)]
    [SerializeField] private float pitchMax = 1.05f;

    // ћетод, который сам знает, как себ€ настроить и воспроизвести на источнике
    public void Play(AudioSource _source)
    {
        if (clips == null 
            || clips.Length == 0)
        {
            return;
        }

        // ¬ыбираем случайный клип, если их несколько
        // Ќапример, если это шаги или удары
        _source.clip = clips[Random.Range(0, clips.Length)];
        _source.volume = volume;
        _source.pitch = Random.Range(pitchMin, pitchMax);

        _source.PlayOneShot(_source.clip);
    }
}