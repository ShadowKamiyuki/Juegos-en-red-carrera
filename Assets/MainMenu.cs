using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private NetworkSceneManagerDefault sceneManager;

    [Header("Team")]
    [SerializeField] private TeamManager teamManager;

    [Header("Team Buttons")]
    [SerializeField] private Button team1Button;
    [SerializeField] private Button team2Button;

    [Header("Network Buttons")]
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject joinButton;

    [Header("Game")]
    [SerializeField] private int gameSceneIndex = 1;

    public static int PlayerTeam { get; private set; }

    private int selectedTeam = 0;

    private const string SessionName = "Match_1";

    private void Start()
    {
        // Al entrar todavía no elegimos equipo
        selectedTeam = 0;
        PlayerTeam = 0;

        // Host y Join empiezan ocultos
        if (hostButton != null)
            hostButton.SetActive(false);

        if (joinButton != null)
            joinButton.SetActive(false);

        // Los botones de equipo empiezan habilitados
        if (team1Button != null)
            team1Button.interactable = true;

        if (team2Button != null)
            team2Button.interactable = true;
    }

    // =========================
    // EQUIPO 1
    // =========================

    public void SelectTeam1()
    {
        SelectTeam(1);
    }

    // =========================
    // EQUIPO 2
    // =========================

    public void SelectTeam2()
    {
        SelectTeam(2);
    }

    // =========================
    // SELECCIONAR EQUIPO
    // =========================

    private void SelectTeam(int team)
    {
        if (selectedTeam != 0)
        {
            Debug.Log("Ya elegiste un equipo.");
            return;
        }

        selectedTeam = team;
        PlayerTeam = team;

        Debug.Log("Elegiste Equipo " + team);

        // Bloqueamos los botones de equipo
        if (team1Button != null)
            team1Button.interactable = false;

        if (team2Button != null)
            team2Button.interactable = false;

        // Ahora aparecen Host y Join
        if (hostButton != null)
            hostButton.SetActive(true);

        if (joinButton != null)
            joinButton.SetActive(true);

        Debug.Log("Ahora podés Hostear o Unirte.");
    }

    // =========================
    // HOST
    // =========================

    public async void CreateGame()
    {
        if (selectedTeam == 0)
        {
            Debug.LogWarning("Primero tenés que elegir un equipo.");
            return;
        }

        if (runner == null)
        {
            Debug.LogError("NetworkRunner no está asignado.");
            return;
        }

        Debug.Log("Creando servidor...");

        StartGameResult result = await runner.StartGame(
            new StartGameArgs
            {
                GameMode = GameMode.Host,
                SessionName = SessionName,

                // IMPORTANTE:
                // NO cargamos Game todavía.
                SceneManager = sceneManager
            });

        if (!result.Ok)
        {
            Debug.LogError(
                "No se pudo crear el servidor: " +
                result.ShutdownReason
            );

            return;
        }

        Debug.Log("Servidor creado.");

        // Ahora que Fusion está conectado,
        // registramos al Host en su equipo.
        if (teamManager != null)
        {
            teamManager.SelectTeam(selectedTeam);
        }
        else
        {
            Debug.LogError("TeamManager no está asignado.");
        }
    }

    // =========================
    // JOIN
    // =========================

    public async void JoinGame()
    {
        if (selectedTeam == 0)
        {
            Debug.LogWarning("Primero tenés que elegir un equipo.");
            return;
        }

        if (runner == null)
        {
            Debug.LogError("NetworkRunner no está asignado.");
            return;
        }

        Debug.Log("Conectando al servidor...");

        StartGameResult result = await runner.StartGame(
            new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = SessionName,

                // Tampoco cargamos Game todavía.
                SceneManager = sceneManager
            });

        if (!result.Ok)
        {
            Debug.LogError(
                "No se pudo conectar: " +
                result.ShutdownReason
            );

            return;
        }

        Debug.Log("Cliente conectado.");

        // Registramos al cliente en su equipo.
        if (teamManager != null)
        {
            teamManager.SelectTeam(selectedTeam);
        }
        else
        {
            Debug.LogError("TeamManager no está asignado.");
        }
    }

    // =========================
    // CARGAR GAME
    // =========================

    public void LoadGameScene()
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner no está asignado.");
            return;
        }

        if (!runner.IsRunning)
        {
            Debug.LogWarning("Fusion todavía no está conectado.");
            return;
        }

        if (!runner.IsServer)
        {
            Debug.LogWarning("Solo el Host puede iniciar la partida.");
            return;
        }

        if (sceneManager == null)
        {
            Debug.LogError("NetworkSceneManagerDefault no está asignado.");
            return;
        }

        Debug.Log("Cargando escena Game...");

        runner.LoadScene(
    SceneRef.FromIndex(gameSceneIndex)
        );
    }

    // =========================
    // SALIR
    // =========================

    public void QuitGame()
    {
        Application.Quit();
    }
}