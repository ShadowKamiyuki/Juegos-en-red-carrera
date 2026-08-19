public class MainMenuState : IAppState
{
    private readonly IAppStateMachine stateMachine;
    private IMainMenuInstaller installer;

    public MainMenuState(IAppStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        installer = ServiceLocator.Get<IMainMenuInstaller>();

        if (installer != null)
        {
            installer.Init(stateMachine);
        }
    }

    public void Exit()
    {
        installer.Dispose();
    }
}
