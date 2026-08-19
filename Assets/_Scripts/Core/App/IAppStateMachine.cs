using System;

public interface IAppStateMachine
{
    AppState CurrentState { get; }

    event Action<AppState> OnStateChanged;

    void SetState(AppState newState);

    LoadingRequest ConsumePendingRequest();
    void RequestSceneChange(LoadingRequest request);
}