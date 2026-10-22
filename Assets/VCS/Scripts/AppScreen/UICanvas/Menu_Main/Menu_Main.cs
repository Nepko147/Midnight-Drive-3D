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
        playButton.AddListener(HandlePlayPressed);
        upgradesButton.AddListener(HandleUpgradesPressed);
        settingsButton.AddListener(HandleSettingsPressed);
        quitButton.AddListener(HandleQuitPressed);
    }

    private void OnDisable()
    {
        playButton.RemoveListener(HandlePlayPressed);
        upgradesButton.RemoveListener(HandleUpgradesPressed);
        settingsButton.RemoveListener(HandleSettingsPressed);
        quitButton.RemoveListener(HandleQuitPressed);
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
        Application.Quit();
    }
}
