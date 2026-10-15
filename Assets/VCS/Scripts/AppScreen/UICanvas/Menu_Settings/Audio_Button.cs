using System;
using UnityEngine;
using UnityEngine.UI;

public class Audio_Button : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image image;
    [SerializeField] private Sprite muteSprite;
    [SerializeField] private Sprite unmuteSprite;

    [SerializeField] private Sprite[] spriteArray;

    private float value;

    public float Value
    {
        get => value;

        set
        {
            this.value = Mathf.Clamp01(value); // √арантируем, что значение будет в диапозоне от 0 до 1
            UpdateVisuals();
        }
    }

    public event Action OnClicked;

    public void OnClick() => OnClicked?.Invoke();

    public void SetMuteVisual(bool _isMuted)
    {
        if (image != null)
        {
            image.sprite = _isMuted ? muteSprite : unmuteSprite;
        }
    }

    private void UpdateVisuals()
    {
        if (spriteArray != null && spriteArray.Length > 0 && image != null)
        {
            int index = Mathf.Clamp((int)Mathf.Ceil((spriteArray.Length - 1) * value), 0, spriteArray.Length - 1);
            image.sprite = spriteArray[index];
        }
    }
}
