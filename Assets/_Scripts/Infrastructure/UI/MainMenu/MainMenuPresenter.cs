public class MainMenuPresenter
{
    private readonly MainMenuView _view;
    private readonly IAppStateMachine _stateMachine;
    private INetworkService network;

    public MainMenuPresenter(MainMenuView view, IAppStateMachine stateMachine)
    {
        _view = view;
        _stateMachine = stateMachine;

        // despues se puede intentar inyectarlo por el constructor si no da problemas
        network = ServiceLocator.Get<INetworkService>();
    }

    public void Initialize()
    {
        _view.OnStartClicked += OnStartClicked;
        _view.OnOptionsClicked += OnOptionsClicked;
        _view.OnQuitClicked += OnQuitClicked;
        _view.OnHostClicked += OnHostClicked;
        _view.OnJoinClicked += OnJoinClicked;
        _view.ShowMainMenu();
    }

    public void Dispose()
    {
        _view.OnStartClicked -= OnStartClicked;
        _view.OnOptionsClicked -= OnOptionsClicked;
        _view.OnQuitClicked -= OnQuitClicked;
        _view.OnHostClicked -= OnHostClicked;
        _view.OnJoinClicked -= OnJoinClicked;
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
        _view.ShowNetwork();
    }

    private void OnHostClicked()
    {
        network.StartGameHost();
        
        // despues comprobar como continuar la escena
        var request = new LoadingRequest(
            load: new[] { "Game" },
            unload: new[] { "MainMenu" },
            nextState: AppState.Gameplay
        );

        _stateMachine.RequestSceneChange(request);
    }

    private void OnJoinClicked()
    {
        network.StartGameClient();
    }
}
