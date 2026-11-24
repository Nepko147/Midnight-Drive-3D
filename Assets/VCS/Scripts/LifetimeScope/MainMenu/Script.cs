using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LifetimeScope_MainMenu : LifetimeScope
{
    [SerializeField] private MusicAsset menuMusic;

    protected override void Configure(IContainerBuilder _builder)
    {
        _builder.RegisterEntryPoint<MainMenuEntryPoint>()
            .WithParameter(menuMusic);
    }
}
