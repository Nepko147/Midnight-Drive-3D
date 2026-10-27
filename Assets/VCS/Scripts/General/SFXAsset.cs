using UnityEngine;

[CreateAssetMenu(fileName = "NewSFXAsset", menuName = "Audio/SFX Asset")]
public class SFXAsset : ScriptableObject
{
    [SerializeField] private AudioClip[] clips;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    // Задаём динамику. Даже если у нас всего один звук, он всё рано
    // будет звучать по разному, при каждом воспроизведении
    [Range(0.1f, 2f)]
    [SerializeField] private float pitchMin = 0.95f;
    [Range(0.1f, 2f)]
    [SerializeField] private float pitchMax = 1.05f;

    // Метод, который сам знает, как себя настроить и воспроизвести на источнике
    public void Play(AudioSource _source)
    {
        if (clips == null 
            || clips.Length == 0)
        {
            return;
        }

        // Останавливаем источник на случай, если он переиспользуется
        _source.Stop();

        // Выбираем случайный клип, если их несколько
        // Например, если это шаги или удары
        _source.clip = clips[Random.Range(0, clips.Length)];
        _source.volume = volume;
        _source.pitch = Random.Range(pitchMin, pitchMax);

        _source.Play();
    }
}