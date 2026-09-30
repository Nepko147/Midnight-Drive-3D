using UnityEngine;

[CreateAssetMenu(fileName = "ButtonVisual", menuName = "UI/Button Visual Data")]

public class ButtonVisualData : ScriptableObject
{
    [Header("State Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite highlightedSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("Fade Settings")]
    [Tooltip("Duration of the smooth transition between sprites (sec)")]
    [SerializeField, Min(0f)] private float transitionDuration = 0.1f;

    public Sprite NormalSprite => normalSprite;
    public Sprite HighlightedSprite => highlightedSprite;
    public Sprite PressedSprite => pressedSprite;
    public float TransitionDuration => transitionDuration;
}