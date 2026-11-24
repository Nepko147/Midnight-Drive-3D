using UnityEngine;
using VContainer;

[RequireComponent(typeof(UI_Button))]
public class UI_Button_Audio : MonoBehaviour
{
    [SerializeField] private SFXAsset pointerEnterSound;
    [SerializeField] private SFXAsset pointerDownSound;

    private UI_Button button;
    private AudioManager audioManager;

    // VContainer сам вызовет этот метод и передаст AudioManager
    [Inject]
    public void Construct (AudioManager _audioManager)
    {
        audioManager = _audioManager;
    }

    private void Awake()
    {
        button = GetComponent<UI_Button>();
    }

    private void OnEnable()
    {
        button.OnPointerEntered += PlayEnterSound;
        button.OnPointerPressed += PlayDownSound;
    }

    private void OnDisable()
    {
        button.OnPointerEntered -= PlayEnterSound;
        button.OnPointerPressed -= PlayDownSound;
    }

    private void PlayEnterSound()
    {
        if (pointerEnterSound != null)
        {
            audioManager?.SFX_Play(pointerEnterSound);
        }
    }

    private void PlayDownSound()
    {
        if (pointerDownSound != null)
        {
            audioManager?.SFX_Play(pointerDownSound);
        }
    }
}
