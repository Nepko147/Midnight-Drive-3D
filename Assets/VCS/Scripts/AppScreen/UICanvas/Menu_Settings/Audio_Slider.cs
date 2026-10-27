using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Audio_Slider : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("Visualizer Link")]
    [SerializeField] private UI_FillSpriteVisualizer fillVisualizer;

    // Событие, на которое подпишется контроллер настроек
    public event Action<float> OnValueChanged;
    
    private RectTransform rectTransform;
    private Image image;
    private float value;

    public float Value
    {
        get => value;

        set
        {
            this.value = Mathf.Clamp01(value); // Гарантируем, что значение будет в диапозоне от 0 до 1

            if (fillVisualizer != null)
            {
                fillVisualizer.UpdateVisual(this.value);
            }
        }
    }
    
    private void Awake()
    {  
        image = GetComponent<Image>();
        rectTransform = image.rectTransform;
    }

    public void OnPointerDown(PointerEventData _eventData) => HandleInput(_eventData);
    public void OnDrag(PointerEventData _eventData) => HandleInput(_eventData);

    private void HandleInput(PointerEventData _eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, _eventData.position, _eventData.pressEventCamera, out Vector2 _localMousePos))
        {
            var _localLeftEdge = rectTransform.rect.xMin + image.raycastPadding.x;
            var _totalWidth = rectTransform.rect.width - (image.raycastPadding.x + image.raycastPadding.z);

            var _normalizedX = (_localMousePos.x - _localLeftEdge) / _totalWidth;
            Value = Mathf.Clamp01(_normalizedX);
            
            // Оповещаем систему об изменении значения
            OnValueChanged?.Invoke(Value);
        }
    }
}
