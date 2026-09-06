using Fusion;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private NetworkSceneManagerDefault sceneManager;

    [Header("Team")]
    [SerializeField] private TeamManager teamManager;

    [Header("Game")]
    [SerializeField] private int gameSceneIndex = 1;

    // Equipo que eligió ESTE jugador
    public static int PlayerTeam { get; private set; }

    private int selectedTeam = 1;

    private const string SessionName = "Match_1";

    private void Start()
    {
        PlayerTeam = 1;
        selectedTeam = 1;
    }

    public void SelectTeam1()
    {
        selectedTeam = 1;
        PlayerTeam = 1;

        Debug.Log("Elegiste Equipo 1");

        if (runner != null && runner.IsRunning)
        {
            teamManager.SelectTeam(1);
        }
    }

    public void SelectTeam2()
    {
        selectedTeam = 2;
        PlayerTeam = 2;

        Debug.Log("Elegiste Equipo 2");

        if (runner != null && runner.IsRunning)
        {
            teamManager.SelectTeam(2);
        }
    }

    public async void CreateGame()
    {
        Debug.Log("Creando servidor...");

        StartGameResult result = await runner.StartGame(
            new StartGameArgs
            {
                GameMode = GameMode.Host,
                SessionName = SessionName,
                Scene = SceneRef.FromIndex(gameSceneIndex),
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

        // El Host ocupa un lugar de su equipo
        if (teamManager != null)
        {
            teamManager.SelectTeam(selectedTeam);
        }
    }

    public async void JoinGame()
    {
        Debug.Log("Conectando al servidor...");

        StartGameResult result = await runner.StartGame(
            new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = SessionName,
                Scene = SceneRef.FromIndex(gameSceneIndex),
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

        // El Cliente ocupa un lugar de su equipo
        if (teamManager != null)
        {
            teamManager.SelectTeam(selectedTeam);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}