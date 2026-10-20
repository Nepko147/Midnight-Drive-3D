using UnityEngine;

public class ControlScene_Script : MonoBehaviour
{
    [SerializeField] private MusicAsset menuTheme;

    private void Start()
    {
        AudioManager.SingleOnScene.Music_Play(menuTheme);
    }
}
