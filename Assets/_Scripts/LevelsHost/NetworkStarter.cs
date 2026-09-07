using Fusion;
using UnityEngine;

public class NetworkStarter : MonoBehaviour
{
    private NetworkRunner runner;

    async void Start()
    {
        runner = gameObject.AddComponent<NetworkRunner>();

        runner.ProvideInput = true;

        NetworkSceneManagerDefault sceneManager =
            gameObject.AddComponent<NetworkSceneManagerDefault>();

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = "MiPartida",
            SceneManager = sceneManager
        });
    }
}