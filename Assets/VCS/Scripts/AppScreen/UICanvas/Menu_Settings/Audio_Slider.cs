using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Audio_Slider : MonoBehaviour
{
    [Header("Audio Setup")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string audioMixer_exposedParamName; // Имя парамметра из микшера
    [SerializeField] private string PlayerPreferenceKey; // Имя параметра для сохранения в PlayerPrefs

    [Header("UI Elements")]
    [SerializeField] private Sprite[] spriteArray;

    private Image image;

    private RectTransform rectTransform;

    private float value;

    private float VALUE_DEFAULT = 0.6f;

    public float Value
    {
        get
        {
            return (value);
        }
        set
        {
            this.value = value;

            var _ind = (int)Mathf.Ceil((spriteArray.Length - 1) * value);
            image.sprite = spriteArray[_ind];
        }
    }

    public void OnClick()
    {
        var _parentCanvas = GetComponentInParent<Canvas>();
        var _camera = _parentCanvas.worldCamera;

        // Берем позицию мыши в пикселях экрана
        Vector2 _screenMousePos = Input.mousePosition;

        // Переводим её в локальное пространство БЕЗ явного указания камеры.
        // Передача null в качестве камеры легитимна только если Canvas в режиме Overlay
        // Иначе, надо "добыть" камеру
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, _screenMousePos, _camera, out Vector2 localMousePos))
        {
            // Локальные границы кнопки
            float localLeftEdge = rectTransform.rect.xMin + image.raycastPadding.x; // Левый край
            float localRightEdge = rectTransform.rect.xMax - image.raycastPadding.z; // Правый край
            float totalWidth = rectTransform.rect.width - (image.raycastPadding.x + image.raycastPadding.z);

            // Вычисляем, где кликнули относительно левого края (от 0.0 до 1.0)
            float normalizedX = (localMousePos.x - localLeftEdge) / totalWidth;
            normalizedX = Mathf.Clamp01(normalizedX);

            Value = normalizedX;

            // Сохраняем состояние, чтобы настройки не слетали при перезапуске
            PlayerPrefs.SetFloat(PlayerPreferenceKey, Value);
            PlayerPrefs.Save();

            ApplyMixerState();
        }
    }

    private void ApplyMixerState()
    {
        var _mixerValue = Mathf.Lerp(-30f, 20f, Value);
        audioMixer.SetFloat(audioMixer_exposedParamName, _mixerValue);
    }
    
    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = image.rectTransform;
    }

    private void Start()
    {
        Value = PlayerPrefs.GetFloat(PlayerPreferenceKey, VALUE_DEFAULT);

        ApplyMixerState();
    }
}
