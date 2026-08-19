using System.Collections.Generic;

public class LoadingRequest
{
    public IReadOnlyList<string> ScenesToLoad => _scenesToLoad;
    public IReadOnlyList<string> ScenesToUnload => _scenesToUnload;
    public AppState NextState { get; }

    private readonly List<string> _scenesToLoad = new();
    private readonly List<string> _scenesToUnload = new();

    public LoadingRequest(IEnumerable<string> load, IEnumerable<string> unload, AppState nextState)
    {
        _scenesToLoad.AddRange(load);
        _scenesToUnload.AddRange(unload);
        NextState = nextState;
    }
}
