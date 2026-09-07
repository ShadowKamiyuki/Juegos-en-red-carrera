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

    [Header("Team Checks - Local Selection")]
    [SerializeField] private GameObject team1Check1;
    [SerializeField] private GameObject team1Check2;
    [SerializeField] private GameObject team2Check1;
    [SerializeField] private GameObject team2Check2;

    [Header("Network Buttons")]
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject joinButton;

    [Header("Game")]
    [SerializeField] private int gameSceneIndex = 1;

    [Header("Colores botones")]
    [SerializeField] private Color normalTeamColor = Color.white;
    [SerializeField] private Color disabledTeamColor = Color.gray;

    public static int PlayerTeam { get; private set; }

    private int selectedTeam = 0;

    private const string SessionName = "Match_1";

    private void Start()
    {
        selectedTeam = 0;
        PlayerTeam = 0;

        SetAllChecks(false);

        // Ocultar Host y Join
        if (hostButton != null)
            hostButton.SetActive(false);

        if (joinButton != null)
            joinButton.SetActive(false);

        // Botones de equipo normales
        SetTeamButton(team1Button, true);
        SetTeamButton(team2Button, true);
    }
    private void UpdateTeamButtons()
    {
        if (teamManager == null)
            return;

        // ==================================================
        // SI ESTE JUGADOR YA ELIGIÓ UN EQUIPO
        // ==================================================

        if (PlayerTeam != 0)
        {
            SetTeamButton(team1Button, false);
            SetTeamButton(team2Button, false);

            return;
        }

        // ==================================================
        // TODAVÍA NO ESTÁ CONECTADO
        // ==================================================

        if (!teamManager.IsNetworkReady)
        {
            SetTeamButton(team1Button, true);
            SetTeamButton(team2Button, true);

            return;
        }

        // ==================================================
        // YA ESTÁ CONECTADO
        // ==================================================

        // Equipo 1
        if (teamManager.IsTeamFull(1))
            SetTeamButton(team1Button, false);
        else
            SetTeamButton(team1Button, true);

        // Equipo 2
        if (teamManager.IsTeamFull(2))
            SetTeamButton(team2Button, false);
        else
            SetTeamButton(team2Button, true);
    }
    private void SetTeamButton(Button button, bool enabled)
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
    private void Update()
    {
        UpdateTeamButtons();
    }
    // ==================================================
    // EQUIPO 1
    // ==================================================

    public void SelectTeam1()
    {
        SelectTeam(1);
    }

    // ==================================================
    // EQUIPO 2
    // ==================================================

    public void SelectTeam2()
    {
        SelectTeam(2);
    }

    // ==================================================
    // SELECCIONAR EQUIPO
    // ==================================================

    private void SelectTeam(int team)
    {
        if (selectedTeam != 0)
        {
            Debug.LogWarning("Ya elegiste un equipo.");
            return;
        }

        selectedTeam = team;
        PlayerTeam = team;

        Debug.Log("Elegiste Equipo " + team);

        // ==========================================
        // MOSTRAR LA CHECK DEL EQUIPO ELEGIDO
        // ==========================================

        if (team == 1)
        {
            // Primera posición del Equipo 1
            if (team1Check1 != null)
                team1Check1.SetActive(true);

            if (team1Check2 != null)
                team1Check2.SetActive(false);

            if (team2Check1 != null)
                team2Check1.SetActive(false);

            if (team2Check2 != null)
                team2Check2.SetActive(false);
        }
        else
        {
            // Primera posición del Equipo 2
            if (team2Check1 != null)
                team2Check1.SetActive(true);

            if (team2Check2 != null)
                team2Check2.SetActive(false);

            if (team1Check1 != null)
                team1Check1.SetActive(false);

            if (team1Check2 != null)
                team1Check2.SetActive(false);
        }

        // ==========================================
        // BLOQUEAR LOS DOS BOTONES
        // ==========================================

        if (team1Button != null)
            team1Button.interactable = false;

        if (team2Button != null)
            team2Button.interactable = false;

        // ==========================================
        // MOSTRAR HOST Y JOIN
        // ==========================================

        if (hostButton != null)
            hostButton.SetActive(true);

        if (joinButton != null)
            joinButton.SetActive(true);

        Debug.Log("Host y Join ahora están visibles.");
    }

    // ==================================================
    // HOST
    // ==================================================

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
                SceneManager = sceneManager
            }
        );

        if (!result.Ok)
        {
            Debug.LogError(
                "No se pudo crear el servidor: " +
                result.ShutdownReason
            );

            return;
        }

        Debug.Log("HOST CONECTADO.");

        // ==========================================
        // REGISTRAR HOST EN SU EQUIPO
        // ==========================================

        if (teamManager != null)
        {
            teamManager.SelectTeam(selectedTeam);
        }
        else
        {
            Debug.LogError("TeamManager no está asignado.");
        }
    }

    // ==================================================
    // JOIN
    // ==================================================

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
                SceneManager = sceneManager
            }
        );

        if (!result.Ok)
        {
            Debug.LogError(
                "No se pudo conectar: " +
                result.ShutdownReason
            );

            return;
        }

        Debug.Log("CLIENTE CONECTADO.");

        // ==========================================
        // REGISTRAR CLIENTE EN SU EQUIPO
        // ==========================================

        if (teamManager != null)
        {
            teamManager.SelectTeam(selectedTeam);
        }
        else
        {
            Debug.LogError("TeamManager no está asignado.");
        }
    }

    // ==================================================
    // OCULTAR LAS 4 CHECKS
    // ==================================================

    private void SetAllChecks(bool state)
    {
        if (team1Check1 != null)
            team1Check1.SetActive(state);

        if (team1Check2 != null)
            team1Check2.SetActive(state);

        if (team2Check1 != null)
            team2Check1.SetActive(state);

        if (team2Check2 != null)
            team2Check2.SetActive(state);
    }

    // ==================================================
    // CARGAR GAME
    // ==================================================

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
            Debug.LogWarning(
                "Solo el Host puede iniciar la partida."
            );

            return;
        }

        Debug.Log(
            "El Host está listo para cargar la escena Game."
        );

        // Dejamos la carga de escena para el siguiente paso,
        // porque depende de la versión de Fusion que estás usando.
    }

    // ==================================================
    // SALIR
    // ==================================================

    public void QuitGame()
    {
        Application.Quit();
    }
}