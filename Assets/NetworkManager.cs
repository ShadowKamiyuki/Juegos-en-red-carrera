using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks, INetworkService
{
    private NetworkRunner runner;

    public void Init(NetworkRunner runner)
    {
        this.runner = runner;

        runner.AddCallbacks(this);
    }

    public async void StartGameHost()
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner no está inicializado.");
            return;
        }

        Debug.Log("Creando servidor...");

        StartGameResult result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = "Match_1"
        });

        if (!result.Ok)
        {
            Debug.LogError(
                "Error creando servidor: " +
                result.ShutdownReason
            );

            return;
        }

        Debug.Log("Servidor creado correctamente.");
    }

    public async void StartGameClient()
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner no está inicializado.");
            return;
        }

        Debug.Log("Conectando como cliente...");

        StartGameResult result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = "Match_1"
        });

        if (!result.Ok)
        {
            Debug.LogError(
                "Error conectando al servidor: " +
                result.ShutdownReason
            );

            return;
        }

        Debug.Log("Cliente conectado correctamente.");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Jugador conectado: " + player);
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

    public void OnDisconnectedFromServer(
        NetworkRunner runner,
        NetDisconnectReason reason)
    {
        Debug.Log("Desconectado del servidor: " + reason);
    }

    public void OnConnectFailed(
        NetworkRunner runner,
        NetAddress remoteAddress,
        NetConnectFailedReason reason)
    {
        Debug.LogError("Falló la conexión: " + reason);
    }

    public void OnShutdown(
        NetworkRunner runner,
        ShutdownReason shutdownReason)
    {
        Debug.Log("Network apagado: " + shutdownReason);
    }

    public void OnInput(
        NetworkRunner runner,
        NetworkInput input)
    {
        // NO HAY INPUT DEL PLAYER TODAVÍA
    }

    public void OnInputMissing(
        NetworkRunner runner,
        PlayerRef player,
        NetworkInput input)
    {
        // NO HAY INPUT DEL PLAYER TODAVÍA
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Comenzando carga de escena.");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Escena cargada.");
    }

    public void OnObjectEnterAOI(
        NetworkRunner runner,
        NetworkObject obj,
        PlayerRef player)
    {
    }

    public void OnObjectExitAOI(
        NetworkRunner runner,
        NetworkObject obj,
        PlayerRef player)
    {
    }
    public void OnConnectRequest(
        NetworkRunner runner,
        NetworkRunnerCallbackArgs.ConnectRequest request,
        byte[] token)
    {
        request.Accept();
    }
    public void OnReliableDataReceived(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        ReadOnlySpan<byte> data)
    {
    }

    public void OnReliableDataProgress(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        float progress)
    {
    }
    public void OnSessionListUpdated(
        NetworkRunner runner,
        List<SessionInfo> sessionList)
    {
    }
    public void OnCustomAuthenticationResponse(
        NetworkRunner runner,
        Dictionary<string, object> data)
    {
    }
    public void OnHostMigration(
        NetworkRunner runner,
        HostMigrationToken hostMigrationToken)
    {
    }
}