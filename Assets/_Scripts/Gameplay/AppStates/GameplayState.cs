public class GameplayState : IAppState
{
    private readonly IAppStateMachine stateMachine;
    private IGameplayInstaller installer;

    public GameplayState(IAppStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        installer = ServiceLocator.Get<IGameplayInstaller>();

        if (installer != null)
        {
            installer.Init();
        }
    }

    public void Exit()
    {
        installer.Dispose();
    }
}
