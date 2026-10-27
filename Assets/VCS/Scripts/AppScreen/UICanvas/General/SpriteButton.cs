using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;
using DG.Tweening;

// Паттерн MVP (Model-View-Presenter)
// SpriteButton - это чистый View (представление).
// Он отвечает только за отображение, анимацию и звуки нажатия

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]

public class SpriteButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Data")]   
    private ButtonVisualData visualData_current;

    [Header("Audio")]
    [SerializeField] private SFXAsset pointerEnterSound;
    [SerializeField] private SFXAsset pointerDownSound;
    
    private RectTransform rectTransform;
    private Image image;
    private Button targetButton;
    private ButtonState state = ButtonState.Normal;
    private bool isPointerInside;
    private bool isPointerDown;
   
    [SerializeField] private LocalizedAsset<ButtonVisualData> localizedAsset;
    
    #region Event

    private UnityEvent onClick;

    // OnClick - Публичное свойство для презентера.
    // На случай, если метод OnEnable в презентере вызовется раньше Awake. 
    // Это свойство гарантированно найдет компонент и вернет его ивент без ошибок.
    public Button.ButtonClickedEvent OnClick
    {
        get
        {
            if (targetButton == null)
            {
                targetButton = GetComponent<Button>();
            }

            return targetButton.onClick;
        }
    }

    private Tween fadeTween;

    public void AddListener(UnityAction _action)
    {
        // Инициализируем в Awake или проверяем на null, если подписка происходит раньше Awake
        if (onClick == null)
        {
            onClick = GetComponent<Button>().onClick;
        }

        onClick.AddListener(_action);
    }

    public void RemoveListener(UnityAction _action) =>  onClick?.RemoveListener(_action);

    #endregion

    #region Input
    // Обработка ввода
    public void OnPointerEnter(PointerEventData _eventData)
    {
        isPointerInside = true;

        if (pointerEnterSound != null)
        {
            AudioManager.SingleOnScene.SFX_Play(pointerEnterSound);
        }

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

        if (pointerDownSound != null)
        {
            AudioManager.SingleOnScene.SFX_Play(pointerDownSound);
        }

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
    #endregion

    // Переходы состояний 
    private void SetState(ButtonState _newState)
    {
        if (state != _newState)
        {
            state = _newState;
            var _targetSprite = GetSpriteForState(_newState);

            // Прерываем предыдущую анимацию, если она идёт
            fadeTween?.Kill();

            if (visualData_current.TransitionDuration <= 0f)
            {
                image.sprite = _targetSprite;
                image.DOFade(1f, 0f);
            }
            else
            {
                var _halfDuratioon = visualData_current.TransitionDuration * 0.5f;
                var _crossfadeSequence = DOTween.Sequence();
                _crossfadeSequence.Append(image.DOFade(0f, _halfDuratioon).SetUpdate(true)) // Игнорируем Time.timeScale
                             .AppendCallback(() => image.sprite = _targetSprite)
                             .Append(image.DOFade(1f, _halfDuratioon).SetUpdate(true));

                fadeTween = _crossfadeSequence;
            }
        }
    }

    // RecalculateState - это единственннное место, где решается,
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

    private Sprite GetSpriteForState(ButtonState _state) => _state switch
    {
        ButtonState.Normal => visualData_current.NormalSprite,
        ButtonState.Highlighted => visualData_current.HighlightedSprite,
        ButtonState.Pressed => visualData_current.PressedSprite,
        _ => visualData_current.NormalSprite,
    };

    private void UpdateVisualData(ButtonVisualData _localizedValue)
    {
        if (_localizedValue != null)
        {
            visualData_current = _localizedValue;

            var _newSprite = visualData_current.NormalSprite;
            image.sprite = _newSprite;

            if (_newSprite != null)
            {
                rectTransform.sizeDelta = new Vector2(_newSprite.rect.width, _newSprite.rect.height);
            }
        }
    }

    private void OnEnable()
    {
        // Подписываемся на событие смены языка
        localizedAsset.AssetChanged += UpdateVisualData;
    }

    private void OnDisable()
    {
        localizedAsset.AssetChanged -= UpdateVisualData;
        fadeTween?.Kill();
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();  
        
        if (targetButton = null)
        {
            targetButton = GetComponent<Button>();
        }
    }
}