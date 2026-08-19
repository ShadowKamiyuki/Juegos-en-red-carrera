public class MainMenuPresenter
{
    private readonly MainMenuView _view;
    private readonly IAppStateMachine _stateMachine;

    public MainMenuPresenter(MainMenuView view, IAppStateMachine stateMachine)
    {
        _view = view;
        _stateMachine = stateMachine;
    }

    public void Initialize()
    {
        _view.OnStartClicked += OnStartClicked;
        _view.OnOptionsClicked += OnOptionsClicked;
        _view.OnQuitClicked += OnQuitClicked;
        _view.Show();
    }

    public void Dispose()
    {
        _view.OnStartClicked -= OnStartClicked;
        _view.OnOptionsClicked -= OnOptionsClicked;
        _view.OnQuitClicked -= OnQuitClicked;
    }

    private void OnOptionsClicked()
    {
        _view.ShowOptions();
    }

    private void OnQuitClicked()
    {
        UnityEngine.Application.Quit();
    }

    private void OnStartClicked()
    {
        var request = new LoadingRequest(
            load: new[] { "Game"},
            unload: new[] { "MainMenu" },
            nextState: AppState.Gameplay
        );

        _stateMachine.RequestSceneChange(request);
    }
}
