using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_FillSpriteVisualizer : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] sprites;

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
    }

    /// <summary>
    /// Обновляет спрайт на основе нормализованного значения (от 0.0 до 1.0)
    /// </summary>
    public void UpdateVisual(float normalizedValue)
    {
        if (targetImage == null || sprites == null || sprites.Length == 0) return;

        normalizedValue = Mathf.Clamp01(normalizedValue);

        // Масштабируем значение на длину массива и вычисляем индекс
        int index = Mathf.Clamp((int)Mathf.Ceil((sprites.Length - 1) * normalizedValue), 0, sprites.Length - 1);
        targetImage.sprite = sprites[index];
    }
}