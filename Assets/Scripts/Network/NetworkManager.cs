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
    public static event Action OnConnectedToGame;
    public static event Action OnConnectionFailed;
    [SerializeField] private NetworkSceneManagerDefault sceneManager;
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private NetworkPrefabRef playerPrefab;
    private InputSystemActions inputSystemActions;
    public Transform[] spawnPoints;

    [Header("Sounds")]
    [SerializeField] private AudioDefinition buttonSound;
    [SerializeField] private AudioDefinition errorCreateServ;
    [SerializeField] private AudioDefinition playerLeft;
    [SerializeField] private AudioDefinition playerSpawn;
    private IAudioService audioService;
    private void Start()
    {
        audioService = ServiceLocator.Get<IAudioService>();
    }
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
            SessionName = sessionName,
            PlayerCount = 4,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        });
        if (audioService != null && buttonSound != null)
        {
            audioService.PlaySFX(buttonSound);
        }

        if (!result.Ok)
        {
            Debug.LogError(
                "Error creando servidor: " +
                result.ShutdownReason
            );

            OnConnectionFailed?.Invoke();

            if (audioService != null && errorCreateServ != null)
            {
                audioService.PlaySFX(errorCreateServ);
            }

            return;
        }

        Debug.Log("Servidor creado correctamente.");

        OnConnectedToGame?.Invoke();
    }

    public async void StartGameClient(string sessionName)
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner no está inicializado.");
            OnConnectionFailed?.Invoke();
            if (audioService != null && errorCreateServ != null)
            {
                audioService.PlaySFX(errorCreateServ);
            }
            return;
        }

        runner.ProvideInput = true;

        Debug.Log("Conectando como cliente...");

        StartGameResult result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = sessionName,
            PlayerCount = 4,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        });
        if (audioService != null && buttonSound != null)
        {
            audioService.PlaySFX(buttonSound);
        }

        if (!result.Ok)
        {
            Debug.LogError(
                "Error conectando al servidor: " +
                result.ShutdownReason
            );

            OnConnectionFailed?.Invoke();

            if (audioService != null && errorCreateServ != null)
            {
                audioService.PlaySFX(errorCreateServ);
            }

            return;
        }

        Debug.Log("Cliente conectado correctamente.");

        OnConnectedToGame?.Invoke();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);

            runner.SetPlayerObject(player, playerObject);

            Debug.Log("Player " + player + " spawneado y asignado correctamente.");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
            return;

        Debug.Log("Jugador desconectado: " + player);

        
        TeamManager teamManager = FindFirstObjectByType<TeamManager>();

        if (teamManager != null)
        {
            teamManager.RemoveDisconnectedPlayer(player);
        }

        NetworkObject playerObject = runner.GetPlayerObject(player);

        if (audioService != null && playerLeft != null)
        {
            audioService.PlaySFX(playerLeft);
        }

        if (playerObject != null)
        {
            Debug.Log(
                "Despawneando personaje de " + player
            );

            runner.Despawn(playerObject);
        }
        else
        {
            Debug.LogWarning(
                "No se encontró PlayerObject para " + player
            );
        }
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

        // Si estamos en el menú, no buscamos SpawnPoints.
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.Log("Estamos en el menú. No se buscan SpawnPoints.");
            return;
        }

        GameObject[] spawnObjects =
            GameObject.FindGameObjectsWithTag("PlayerSpawn");

        if (spawnObjects.Length < 4)
        {
            Debug.LogError(
                "La escena necesita 4 SpawnPoints con el tag PlayerSpawn."
            );

            return;
        }

        TeamManager teamManager =
            FindFirstObjectByType<TeamManager>();

        if (teamManager == null)
        {
            Debug.LogError(
                "No se encontró TeamManager al cargar el nivel."
            );

            return;
        }

        int spawnIndex = 0;

        foreach (PlayerRef player in runner.ActivePlayers)
        {
            NetworkObject playerObject =
                runner.GetPlayerObject(player);


            if (playerObject == null)
            {
                Transform spawnPoint =
                    spawnObjects[spawnIndex].transform;

                playerObject = runner.Spawn(
                    playerPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation,
                    player
                );

                runner.SetPlayerObject(
                    player,
                    playerObject
                );

                Debug.Log("Player " + player + " creado en SpawnPoint " + spawnIndex);
                if (audioService != null && playerSpawn != null)
                {
                    audioService.PlaySFX(playerSpawn);
                }
            }
            else
            {
                Debug.Log("Player " + player + " ya existe.");
            }


            Transform currentSpawnPoint =
                spawnObjects[spawnIndex].transform;

            playerObject.transform.SetPositionAndRotation(
                currentSpawnPoint.position,
                currentSpawnPoint.rotation
            );



            int team = teamManager.GetPlayerTeam(player);

            Player playerComponent = playerObject.GetComponent<Player>();

            if (playerComponent != null)
            {
                playerComponent.Team = team;

                Debug.Log("Player " + player + " recuperó el Equipo " + team);
            }
            else
            {
                Debug.LogWarning("El PlayerObject de " + player + " no tiene componente Player.");
            }


            spawnIndex++;
        }
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