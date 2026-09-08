
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

    [Header("Colores")]
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
        selectedTeam = 0;
        PlayerTeam = 0;


        // Ocultar Host / Join

        if (hostButton != null)
            hostButton.SetActive(false);

        if (joinButton != null)
            joinButton.SetActive(false);


        // Activar botones de equipo

        SetTeamButton(team1Button, true);
        SetTeamButton(team2Button, true);
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        UpdateTeamButtons();
    }


    // =========================================================
    // BOTONES DE EQUIPO
    // =========================================================

    private void UpdateTeamButtons()
    {
        if (teamManager == null)
            return;


        // Ya eligió equipo.
        if (PlayerTeam != 0)
        {
            SetTeamButton(team1Button, false);
            SetTeamButton(team2Button, false);

            return;
        }


        // Fusion todavía no está listo.

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
    // COLOR / INTERACTABLE
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
    // SELECCIONAR EQUIPO 1
    // =========================================================

    public void SelectTeam1()
    {
        SelectTeam(1);
    }


    // =========================================================
    // SELECCIONAR EQUIPO 2
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


        selectedTeam = team;
        PlayerTeam = team;


        Debug.Log(
            "Elegiste Equipo " + team
        );


        // Desactivar ambos botones.

        SetTeamButton(team1Button, false);
        SetTeamButton(team2Button, false);


        // Mostrar Host / Join.

        if (hostButton != null)
            hostButton.SetActive(true);

        if (joinButton != null)
            joinButton.SetActive(true);
    }


    // =========================================================
    // HOST
    // =========================================================

    public async void CreateGame()
    {
        if (selectedTeam == 0)
        {
            Debug.LogWarning(
                "Primero elegí un equipo."
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


        Debug.Log(
            "Creando HOST..."
        );


        StartGameResult result =
            await runner.StartGame(
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
                "No se pudo crear el HOST: " +
                result.ShutdownReason
            );

            return;
        }


        Debug.Log(
            "HOST CONECTADO."
        );


        // Esperamos a que TeamManager esté listo.

        if (teamManager != null)
        {
            teamManager.SelectTeam(selectedTeam);
        }
        else
        {
            Debug.LogError(
                "TeamManager no está asignado."
            );
        }
    }


    // =========================================================
    // CLIENTE
    // =========================================================

    public async void JoinGame()
    {
        if (selectedTeam == 0)
        {
            Debug.LogWarning(
                "Primero elegí un equipo."
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


        Debug.Log(
            "Conectando como CLIENTE..."
        );


        StartGameResult result =
            await runner.StartGame(
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


        Debug.Log(
            "CLIENTE CONECTADO."
        );


        if (teamManager != null)
        {
            teamManager.SelectTeam(selectedTeam);
        }
        else
        {
            Debug.LogError(
                "TeamManager no está asignado."
            );
        }
    }


    // =========================================================
    // SALIR
    // =========================================================

    public void QuitGame()
    {
        Application.Quit();
    }
}

