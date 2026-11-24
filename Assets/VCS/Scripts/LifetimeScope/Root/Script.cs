using UnityEngine;
using VContainer;
using VContainer.Unity;

// Данный класс живёт на протяжении всей игры.
// Он нужен для хранения глобальных менеджеров.
public class LifetimeScope_Root : LifetimeScope
{
    [SerializeField] private AudioManager audioManagerPrefab;
    [SerializeField] private LocalizationManager localizationManagerPrefab;

    protected override void Configure(IContainerBuilder _builder)
    {
        // 1. C#-классы регистрируем как Singleton
        _builder.Register<ISaveService, JsonSaveService>(Lifetime.Singleton);
        _builder.Register<SaveManager>(Lifetime.Singleton);

        // 2. MonoBehaviour-менеджеры создаём из префабов и делаем бессмертными
        _builder.RegisterComponentInNewPrefab(audioManagerPrefab, Lifetime.Singleton)
               .DontDestroyOnLoad();

        _builder.RegisterComponentInNewPrefab(localizationManagerPrefab, Lifetime.Singleton)
               .DontDestroyOnLoad();
    }
}
