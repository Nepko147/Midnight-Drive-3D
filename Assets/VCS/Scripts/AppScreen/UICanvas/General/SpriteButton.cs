using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]

public abstract class SpriteButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Data")]   
    private ButtonVisualData visualData_current;

    private UnityEvent onClick;
    private RectTransform rectTransform;
    private Image image;
    private ButtonState state = ButtonState.Normal;
    private Coroutine transitionRoutine;
    private bool isPointerInside;
    private bool isPointerDown;
   
    [SerializeField] private LocalizedAsset<ButtonVisualData> localizedAsset;

    private void OnEnable()
    {
        // Подписываемся на событие смены языка
        localizedAsset.AssetChanged += UpdateVisualData;
    }

    private void OnDisable()
    {
        localizedAsset.AssetChanged -= UpdateVisualData;
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        var _button = GetComponent<Button>();
        onClick = _button.onClick;
    }

    // Переходы состояний 
    private void SetState(ButtonState _newState)
    {
        if (state == _newState)
            return;

        state = _newState;
        Sprite _targetSprite = GetSpriteForState(_newState);

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        if (visualData_current.TransitionDuration <= 0f)
        {
            image.sprite = _targetSprite;
        }
        else
        {
            transitionRoutine = StartCoroutine(CrossfadeSprite(_targetSprite, visualData_current.TransitionDuration));
        }
    }

    private Sprite GetSpriteForState(ButtonState _state) => _state switch
    {
        ButtonState.Normal => visualData_current.NormalSprite,
        ButtonState.Highlighted => visualData_current.HighlightedSprite,
        ButtonState.Pressed => visualData_current.PressedSprite,
        _ => visualData_current.NormalSprite,
    };

    // Плавный переход
    private IEnumerator CrossfadeSprite(Sprite _target, float _duration)
    {
        float _half = _duration * 0.5f;

        // Fade out
        yield return FadeAlpha(1f, 0f, _half);
        image.sprite = _target;
        // Fade in
        yield return FadeAlpha(0f, 1f, _half);
    }

    private IEnumerator FadeAlpha(float _from, float _to, float _time)
    {
        float _elapsed = 0f;
        Color _color = image.color;

        while (_elapsed < _time)
        {
            _elapsed += Time.unscaledDeltaTime;
            _color.a = Mathf.Lerp(_from, _to, _elapsed / _time);
            image.color = _color;
            yield return null;
        }

        _color.a = _to;
        image.color = _color;
    }

    // RecalculateState: Единственннное место, где решается,
    // какое состояние должно быть сейчас, а не разбросано по хэндлерам.
    private void RecalculateState()
    {
        if (isPointerDown)
        {
            SetState(ButtonState.Pressed);
        }
        else if (isPointerInside)
        {
            SetState(ButtonState.Highlighted);
        }
        else
        {
            SetState(ButtonState.Normal);
        }
    }

    private void UpdateVisualData(ButtonVisualData _localizedValue)
    {
        if (_localizedValue == null) return;

        visualData_current = _localizedValue;

        var _newSprite = visualData_current.NormalSprite;
        image.sprite = _newSprite;

        var _newSizeDelata_width = _newSprite.rect.width;
        var _newSizeDelata_height = _newSprite.rect.height;
        var _newSizeDelata = new Vector2(_newSizeDelata_width, _newSizeDelata_height);
        rectTransform.sizeDelta = _newSizeDelata;
    }

    // Обработка ввода
    public void OnPointerEnter(PointerEventData _eventData)
    {
        isPointerInside = true;
        AudioManager.SingleOnScene.PlaySFX("Button"); //Временно через строку, пока не определимся с системой хранения звуков
        RecalculateState();
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        isPointerInside = false;
        RecalculateState();
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        isPointerDown = true;
        AudioManager.SingleOnScene.PlaySFX("Button"); //Временно через строку, пока не определимся с системой хранения звуков
        RecalculateState();
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        isPointerDown = false;

        // Клик засчитывается, если курсор всё ещё над кнопкой
        if (isPointerInside) 
        {
            onClick?.Invoke();
        }            

        RecalculateState();
    }

    public virtual void OnClick()
    {
        
    }
}