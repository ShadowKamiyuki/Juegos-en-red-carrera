using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderService : MonoBehaviour, ISceneLoader
{
    public bool IsLoading { get; private set; }
    public float Progress { get; private set; }

    public async Task ProcessRequest(LoadingRequest request, Action<float> onProgress = null)
    {
        if (IsLoading)
            return;

        IsLoading = true;
        Progress = 0f;

        float startTime = Time.unscaledTime;

        foreach (var scene in request.ScenesToUnload)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(scene);
            await AwaitOperation(unloadOp, onProgress);
        }

        foreach (var scene in request.ScenesToLoad)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            await AwaitOperation(loadOp, onProgress);
        }

        Progress = 1f;
        onProgress?.Invoke(1f);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(request.ScenesToLoad[0]));

        IsLoading = false;
    }

    private async Task AwaitOperation(AsyncOperation operation, Action<float> onProgress)
    {
        while (!operation.isDone)
        {
            Progress = Mathf.Clamp01(operation.progress / 0.9f);
            onProgress?.Invoke(Progress);
            await Task.Yield();
        }
    }
}