using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks, INetworkService
{
    private NetworkRunner runner;
    private IInputService inputService;
    [SerializeField] private NetworkObject playerPrefab;
    private Transform spawnPoint;
    public enum InputButton
    {
        Fire = 0
    }
    private void Awake()
    {
        MonoBehaviour[] components = FindObjectsByType<MonoBehaviour>(
            FindObjectsSortMode.None
        );

        foreach (MonoBehaviour component in components)
        {
            if (component is IInputService service)
            {
                inputService = service;
                Debug.Log("InputService encontrado: " + component.gameObject.name);
                break;
            }
        }

        if (inputService == null)
        {
            Debug.LogError("NO se encontró ningún objeto con IInputService");
        }
    }

    public void Init(NetworkRunner runner, IInputService inputService)
    {
        this.runner = runner;

        if (inputService != null)
        {
            this.inputService = inputService;
        }

        runner.AddCallbacks(this);
    }

    public async void StartGameHost()
    {
        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = "Match_1"
        });
    }

    public async void StartGameClient()
    {
        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = "Match_1"
        });
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
            return;

        GameObject spawnObject = GameObject.Find("spawnPoint");

        if (spawnObject == null)
        {
            Debug.LogError("No se encontró spawnPoint");
            return;
        }

        NetworkObject spawnedPlayer = runner.Spawn(
            playerPrefab,
            spawnObject.transform.position,
            spawnObject.transform.rotation,
            player
        );

        Debug.Log("PLAYER CREADO: " + spawnedPlayer.name);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (inputService == null)
        {
            Debug.LogError("inputService sigue siendo NULL");
            return;
        }

        NetworkInputData data = new NetworkInputData();

        data.direction = inputService.Move.normalized;

        Debug.Log("DIRECCION: " + data.direction);

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (!runner.IsServer)
            return;

        GameObject spawnObject = GameObject.Find("spawnPoint");

        if (spawnObject == null)
        {
            Debug.LogError("No se encontró spawnPoint en la escena Game");
            return;
        }

        spawnPoint = spawnObject.transform;

        Debug.Log("SpawnPoint encontrado: " + spawnPoint.name);

        foreach (PlayerRef player in runner.ActivePlayers)
        {
            runner.Spawn(
                playerPrefab,
                spawnPoint.position,
                spawnPoint.rotation,
                player
            );
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
}
