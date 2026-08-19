using System.Collections.Generic;

public class StateFactory
{
    private readonly IAppStateMachine stateMachine;
    private readonly ISceneLoader sceneLoader;
    private readonly LoadingView loadingView;
    private readonly FadeController fadeController;

    public StateFactory(IAppStateMachine stateMachine, ISceneLoader sceneLoader, LoadingView loadingView, FadeController fadeController)
    {
        this.stateMachine = stateMachine;
        this.sceneLoader = sceneLoader;
        this.loadingView = loadingView;
        this.fadeController = fadeController;
    }

    public Dictionary<AppState, IAppState> Create()
    {
        return new Dictionary<AppState, IAppState>
        {
            { AppState.MainMenu, new MainMenuState(stateMachine) },
            { AppState.Loading, new LoadingState(stateMachine, sceneLoader, loadingView, fadeController) },
            { AppState.Gameplay, new GameplayState(stateMachine) },
            { AppState.GameOver, new GameOverState() },
            { AppState.Paused, new PausedState() }
        };
    }
}
