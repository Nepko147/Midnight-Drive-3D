using System;
using UnityEngine;
using UnityEngine.UI;

public class Audio_Button : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image image;
    [SerializeField] private Sprite muteSprite;

    [Header("Visualizer Link (Optional)")]
    [SerializeField] private UI_FillSpriteVisualizer fillVisualizer;

    private float value;

    public float Value
    {
        get => value;

        set
        {
            this.value = Mathf.Clamp01(value); // √арантируем, что значение будет в диапозоне от 0 до 1
            
            if (fillVisualizer != null)
            {
                fillVisualizer.UpdateVisual(this.value);
            }
        }
    }

    public event Action OnClicked;

    public void OnClick() => OnClicked?.Invoke();

    public void SetMuteVisual(bool _isMuted)
    {
        if (image != null
            && _isMuted)
        {
            image.sprite =  muteSprite;
        }
    }
}
