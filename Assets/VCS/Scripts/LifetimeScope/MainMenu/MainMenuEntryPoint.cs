using VContainer.Unity;

public class MainMenuEntryPoint : IStartable
{
    private readonly AudioManager audioManager;
    private readonly MusicAsset menuMusic;

    // VContainer сам подставит зависимости в конструктор
    public MainMenuEntryPoint(AudioManager _audioManager, MusicAsset _menuMusic)
    {
        audioManager = _audioManager;
        menuMusic = _menuMusic;
    }

    public void Start()
    {
        audioManager.Music_Play(menuMusic);
    }
}
