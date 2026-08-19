using UnityEngine;

public class GameStateAudioController : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioDefinition mainMenuMusic;
    [SerializeField] private AudioDefinition gameOverMusic;

    private IAppStateMachine stateMachine;
    private IAudioService audioService;

    public void Init(IAppStateMachine stateMachine, IAudioService audioService)
    {
        this.stateMachine = stateMachine;
        this.audioService = audioService;

        stateMachine.OnStateChanged += HandleStateChanged;
    }

    private void OnDestroy()
    {
        if (stateMachine != null)
            stateMachine.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(AppState state)
    {
        switch (state)
        {
            case AppState.MainMenu:
                Play(mainMenuMusic);
                break;

            case AppState.Loading:
                break;

            case AppState.GameOver:
                Play(gameOverMusic);
                break;

            case AppState.Gameplay:
                break;

            case AppState.Paused:
                break;
        }
    }

    private void Play(AudioDefinition definition)
    {
        if (definition != null)
            audioService.PlayMusic(definition);
    }
}