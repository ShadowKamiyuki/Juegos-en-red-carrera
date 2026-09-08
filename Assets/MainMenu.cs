using Fusion;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

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

    [Header("Colores botones")]
    [SerializeField] private Color normalTeamColor = Color.white;
    [SerializeField] private Color disabledTeamColor = Color.gray;

    public static int PlayerTeam { get; private set; }

    private int selectedTeam = 0;

    private const string SessionName = "Match_1";


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (teamManager == null)
        {
            teamManager = FindFirstObjectByType<TeamManager>();

            if (teamManager == null)
            {
                Debug.LogError("NO EXISTE NINGÚN TEAM MANAGER EN LA ESCENA.");
            }
            else
            {
                Debug.Log("TeamManager encontrado automáticamente.");
            }
        }

        selectedTeam = 0;
        PlayerTeam = 0;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        UpdateTeamButtons();
    }


    // =========================================================
    // ACTUALIZAR BOTONES DE EQUIPO
    // =========================================================

    private void UpdateTeamButtons()
    {
        if (teamManager == null)
            return;


        // Si este jugador ya eligió equipo,
        // no puede cambiarlo
        if (PlayerTeam != 0)
        {
            SetTeamButton(team1Button, false);
            SetTeamButton(team2Button, false);

            return;
        }


        // Si Fusion todavía no está conectado,
        // ambos equipos están disponibles
        if (!teamManager.IsNetworkReady)
        {
            SetTeamButton(team1Button, true);
            SetTeamButton(team2Button, true);

            return;
        }


        // Equipo 1
        SetTeamButton(
            team1Button,
            !teamManager.IsTeamFull(1)
        );


        // Equipo 2
        SetTeamButton(
            team2Button,
            !teamManager.IsTeamFull(2)
        );
    }


    // =========================================================
    // CAMBIAR COLOR / ESTADO BOTÓN
    // =========================================================

    private void SetTeamButton(
        Button button,
        bool enabled)
    {
        if (button == null)
            return;

        button.interactable = enabled;

        ColorBlock colors = button.colors;


        if (enabled)
        {
            colors.normalColor = normalTeamColor;
            colors.highlightedColor = normalTeamColor;
            colors.pressedColor = normalTeamColor;
            colors.selectedColor = normalTeamColor;
        }
        else
        {
            colors.normalColor = disabledTeamColor;
            colors.highlightedColor = disabledTeamColor;
            colors.pressedColor = disabledTeamColor;
            colors.selectedColor = disabledTeamColor;
            colors.disabledColor = disabledTeamColor;
        }


        button.colors = colors;
    }


    // =========================================================
    // BOTÓN EQUIPO 1
    // =========================================================

    public void SelectTeam1()
    {
        SelectTeam(1);
    }


    // =========================================================
    // BOTÓN EQUIPO 2
    // =========================================================

    public void SelectTeam2()
    {
        SelectTeam(2);
    }


    // =========================================================
    // SELECCIONAR EQUIPO
    // =========================================================

    private void SelectTeam(int team)
    {
        if (selectedTeam != 0)
        {
            Debug.LogWarning(
                "Ya elegiste un equipo."
            );

            return;
        }


        if (team != 1 && team != 2)
        {
            Debug.LogWarning(
                "Equipo inválido."
            );

            return;
        }


        selectedTeam = team;
        PlayerTeam = team;


        Debug.Log(
            "Elegiste Equipo " + team
        );


        // Desactivar ambos botones
        SetTeamButton(
            team1Button,
            false
        );

        SetTeamButton(
            team2Button,
            false
        );


        // Mostrar Host y Join
        if (hostButton != null)
            hostButton.SetActive(true);

        if (joinButton != null)
            joinButton.SetActive(true);


        Debug.Log(
            "Host y Join ahora están visibles."
        );
    }


    // =========================================================
    // CREAR HOST
    // =========================================================

    public async void CreateGame()
    {
        if (selectedTeam == 0)
        {
            Debug.LogWarning(
                "Primero tenés que elegir un equipo."
            );

            return;
        }


        if (runner == null)
        {
            Debug.LogError(
                "NetworkRunner no está asignado."
            );

            return;
        }


        if (runner.IsRunning)
        {
            Debug.LogWarning(
                "Fusion ya está ejecutándose."
            );

            return;
        }


        Debug.Log(
            "Creando servidor..."
        );


        StartGameResult result =
            await runner.StartGame(
                new StartGameArgs
                {
                    GameMode = GameMode.Host,
                    SessionName = SessionName,
                    SceneManager = sceneManager,
                    PlayerCount = 4
                }
            );


        if (!result.Ok)
        {
            Debug.LogError(
                "NO SE PUDO CREAR EL HOST. " +
                "Motivo: [" +
                result.ShutdownReason +
                "]"
            );

            return;
        }


        Debug.Log(
            "HOST CONECTADO."
        );


        // Esperar a que TeamManager
        // sea Spawned por Fusion

        await WaitForTeamManager();


        if (teamManager != null)
        {
            teamManager.SelectTeam(
                selectedTeam
            );
        }
        else
        {
            Debug.LogError(
                "TeamManager no está asignado."
            );
        }
    }


    // =========================================================
    // UNIRSE COMO CLIENTE
    // =========================================================

    public async void JoinGame()
    {
        if (selectedTeam == 0)
        {
            Debug.LogWarning(
                "Primero tenés que elegir un equipo."
            );

            return;
        }


        if (runner == null)
        {
            Debug.LogError(
                "NetworkRunner no está asignado."
            );

            return;
        }


        if (runner.IsRunning)
        {
            Debug.LogWarning(
                "Fusion ya está ejecutándose."
            );

            return;
        }


        Debug.Log(
            "Conectando al servidor..."
        );


        StartGameResult result =
            await runner.StartGame(
                new StartGameArgs
                {
                    GameMode = GameMode.Client,
                    SessionName = SessionName,
                    SceneManager = sceneManager,
                    PlayerCount = 4
                }
            );


        if (!result.Ok)
        {
            Debug.LogError(
                "NO SE PUDO CONECTAR. " +
                "Motivo: [" +
                result.ShutdownReason +
                "]"
            );

            return;
        }


        Debug.Log(
            "CLIENTE CONECTADO."
        );


        // Esperar a que TeamManager
        // sea Spawned

        await WaitForTeamManager();


        if (teamManager != null)
        {
            teamManager.SelectTeam(
                selectedTeam
            );
        }
        else
        {
            Debug.LogError(
                "TeamManager no está asignado."
            );
        }
    }


    // =========================================================
    // ESPERAR TEAM MANAGER
    // =========================================================

    private async Task WaitForTeamManager()
    {
        while (teamManager == null)
        {
            teamManager = FindFirstObjectByType<TeamManager>();

            if (teamManager == null)
            {
                await Task.Yield();
            }
        }

        while (!teamManager.IsNetworkReady)
        {
            await Task.Yield();
        }

        Debug.Log("TeamManager está listo para recibir jugadores.");
    }


    // =========================================================
    // CARGAR GAME
    // =========================================================

    public void LoadGameScene()
    {
        if (runner == null)
        {
            Debug.LogError(
                "NetworkRunner no está asignado."
            );

            return;
        }


        if (!runner.IsRunning)
        {
            Debug.LogWarning(
                "Fusion todavía no está conectado."
            );

            return;
        }


        if (!runner.IsServer)
        {
            Debug.LogWarning(
                "Solo el Host puede iniciar la partida."
            );

            return;
        }


        Debug.Log(
            "El Host está listo para cargar la escena Game."
        );


        // Lo hacemos después
    }


    // =========================================================
    // SALIR
    // =========================================================

    public void QuitGame()
    {
        Application.Quit();
    }
}