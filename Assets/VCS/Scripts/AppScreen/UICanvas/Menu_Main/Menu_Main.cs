using UnityEngine;

// Паттерн MVP (Model-View-Presenter)
// Menu_Main - это Presenter (контроллер).
// Он связывает элементы интерфейса с игровой логикой (запуск игры, выход и т.д.)

public class Menu_Main : MonoBehaviour
{
    [SerializeField] private SpriteButton playButton;
    [SerializeField] private SpriteButton upgradesButton;
    [SerializeField] private SpriteButton settingsButton;
    [SerializeField] private SpriteButton quitButton;

    private void OnEnable()
    {   
        playButton.OnClick.AddListener(HandlePlayPressed);
        upgradesButton.OnClick.AddListener(HandleUpgradesPressed);
        settingsButton.OnClick.AddListener(HandleSettingsPressed);
        quitButton.OnClick.AddListener(HandleQuitPressed);
    }

    private void OnDisable()
    {
        playButton.OnClick.RemoveListener(HandlePlayPressed);
        upgradesButton.OnClick.RemoveListener(HandleUpgradesPressed);
        settingsButton.OnClick.RemoveListener(HandleSettingsPressed);
        quitButton.OnClick.RemoveListener(HandleQuitPressed);
    }

    // В MVP подобные классы выступают в роли Контроллера.
    // Его ЕДИНСТВЕННАЯ обязанность — координировать работу меню.
    // Поэтому наличие "разношёрстных" методов
    // не противоречит Single Responsibility Principle (SRP)

    private void HandlePlayPressed()
    {
        // Логика старта игры
    }

    private void HandleUpgradesPressed()
    {
        // Открытие Меню Апгрейдов
    }

    private void HandleSettingsPressed()
    {
        //  Открытие Меню Настроек
    }

    private void HandleQuitPressed()
    {
        // Логика перед закрытием игры (если надо)
        Application.Quit();
    }
}
