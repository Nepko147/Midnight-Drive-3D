using UnityEngine;

[CreateAssetMenu(fileName = "MusicAsset", menuName = "Audio/Music Asset")]
public class MusicAsset : ScriptableObject
{
    [SerializeField] private AudioClip clip;
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;
    [SerializeField] private bool loop = true;

    public AudioClip Clip => clip;
    public float Volume => volume;
    public bool Loop => loop;
}