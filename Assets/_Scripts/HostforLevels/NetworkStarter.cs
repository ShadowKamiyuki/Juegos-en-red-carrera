using Fusion;
using UnityEngine;
// Este codigo esta por las dudas, cualquier cosa se puede borrar
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