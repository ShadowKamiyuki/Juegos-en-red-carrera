using UnityEngine;

public class LoadingState : IAppState
{
    private readonly IAppStateMachine _stateMachine;
    private readonly ISceneLoader _sceneLoader;
    private readonly ILoadingView _loadingView;
    private readonly IFadeService _fadeController;

    public LoadingState(IAppStateMachine stateMachine, ISceneLoader sceneLoader, ILoadingView loadingView, IFadeService fadeController)
    {
        _stateMachine = stateMachine;
        _sceneLoader = sceneLoader;
        _loadingView = loadingView;
        _fadeController = fadeController;
    }

    public async void Enter()
    {
        LoadingRequest request = _stateMachine.ConsumePendingRequest();

        if (request == null)
        {
            Debug.LogError("No LoadingRequest found.");
            return;
        }

        _loadingView.ResetProgress();
        _loadingView.Show();

        await _fadeController.FadeToBlackAsync();

        await _sceneLoader.ProcessRequest(request, _loadingView.SetProgress);

        _loadingView.Hide();

        await _fadeController.FadeFromBlackAsync();

        _stateMachine.SetState(request.NextState);
    }

    public void Exit() { }
}

