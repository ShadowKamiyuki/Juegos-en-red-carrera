public interface IUpdateService
{
    void Register(IUpdatable updatable);
    void Unregister(IUpdatable updatable);
}