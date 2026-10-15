using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Audio_Slider : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] spriteArray;

    // Событие, на которое подпишется контроллер настроек
    public event Action<float> OnValueChanged;

    [Header("Linked Elements")]
    [SerializeField] private Audio_Button linkedButton; // Ссылка на связанную кнопку
    
    private RectTransform rectTransform;
    private float value;

    public float Value
    {
        get => value;

        set
        {
            this.value = Mathf.Clamp01(value); // Гарантируем, что значение будет в диапозоне от 0 до 1
            UpdateVisuals();            
        }
    }
    
    private void Awake()
    {        
        if (image == null)
        {
            image = GetComponent<Image>();
        }

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

    private void UpdateVisuals()
    {
        if (spriteArray != null && spriteArray.Length > 0 && image != null)
        {
            int index = Mathf.Clamp((int)Mathf.Ceil((spriteArray.Length - 1) * value), 0, spriteArray.Length - 1);
            image.sprite = spriteArray[index];
        }
    }
}
