using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static Unity.Collections.Unicode;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private NetworkPrefabRef playerPrefab;
    private InputSystemActions inputSystemActions;
    public Transform[] spawnPoints;

    private void Awake()
    {
        runner.AddCallbacks(this);
        inputSystemActions = new InputSystemActions();
    }

    private void OnEnable()
    {
        inputSystemActions.Enable();
    }

    private void OnDisable()
    {
        inputSystemActions.Disable();
    }

    public async void StartGameHost(string sessionName)
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner no está inicializado.");
            return;
        }
        runner.ProvideInput = true;

        Debug.Log("Creando servidor...");

        StartGameResult result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = sessionName
        });

        if (!result.Ok)
        {
            Debug.LogError("Error creando servidor: " + result.ShutdownReason);

            return;
        }

        Debug.Log("Servidor creado correctamente.");
    }

    public async void StartGameClient(string sessionName)
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner no está inicializado.");
            return;
        }
        runner.ProvideInput = true;

        Debug.Log("Conectando como cliente...");

        StartGameResult result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = sessionName
        });

        if (!result.Ok)
        {
            Debug.LogError("Error conectando al servidor: " + result.ShutdownReason);

            return;
        }

        Debug.Log("Cliente conectado correctamente.");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        TeamManager teamManager = FindFirstObjectByType<TeamManager>();

        if (teamManager != null && runner.IsServer)
        {
            teamManager.RemoveDisconnectedPlayer(player);
        }

        Debug.Log("Jugador desconectado: " + player);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Conectado al servidor.");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("Desconectado del servidor: " + reason);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError("Falló la conexión: " + reason);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Network apagado: " + shutdownReason);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Vector2 direction = inputSystemActions.Gameplay.Move.ReadValue<Vector2>();

        NetworkInputData data = new NetworkInputData
        {
            Direction = direction
        };

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // NO HAY INPUT DEL PLAYER TODAVÍA
    }
    public void LoadGameScene(int sceneIndex)
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner no está inicializado.");
            return;
        }

        if (!runner.IsSceneAuthority)
        {
            Debug.LogWarning("Solo el Host puede cargar la escena.");
            return;
        }

        SceneRef scene = SceneRef.FromIndex(sceneIndex);

        Debug.Log($"Host cargando escena {sceneIndex}");

        runner.LoadScene(scene, LoadSceneMode.Single);
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log($"Fusion comenzó a cargar la escena");
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Escena cargada.");

        if (!runner.IsServer)
            return;

        GameObject[] spawnObjects =
            GameObject.FindGameObjectsWithTag("PlayerSpawn");

        if (spawnObjects.Length < 4)
        {
            Debug.LogError(
                "La escena necesita 4 SpawnPoints con el tag PlayerSpawn."
            );

            return;
        }

        int spawnIndex = 0;

        foreach (PlayerRef player in runner.ActivePlayers)
        {
            NetworkObject playerObject =
                runner.GetPlayerObject(player);

            if (playerObject != null)
                continue;

            Transform spawnPoint =
                spawnObjects[spawnIndex].transform;

            runner.Spawn(
                playerPrefab,
                spawnPoint.position,
                spawnPoint.rotation,
                player
            );

            Debug.Log(
                "Player " + player +
                " spawneado en SpawnPoint " + spawnIndex
            );

            spawnIndex++;
        }
    }


    private int GetSpawnIndex(PlayerRef player)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No hay Spawn Points configurados.");
            return 0;
        }

        return player.RawEncoded % spawnPoints.Length;
    }

    private Vector3 GetSpawnPosition()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");

        if (spawnPoint != null)
            return spawnPoint.transform.position;

        return Vector3.zero;
    }


    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj,PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        request.Accept();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }
}