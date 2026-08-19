using UnityEngine;

public class MainMenuInstaller : MonoBehaviour, IMainMenuInstaller
{
    [SerializeField] private MainMenuView mainMenuView;

    private MainMenuPresenter mainMenuPresenter;

    private void Awake()
    {
        ServiceLocator.Register<IMainMenuInstaller>(this);
    }

    private void OnDestroy()
    {
        if (ServiceLocator.Exists<IMainMenuInstaller>())
            ServiceLocator.Unregister<IMainMenuInstaller>();
    }

    public void Init(IAppStateMachine stateMachine)
    {
        mainMenuPresenter = new MainMenuPresenter(mainMenuView, stateMachine);
        mainMenuPresenter.Initialize();
    }

    public void Dispose()
    {
        mainMenuPresenter?.Dispose();
    }
}