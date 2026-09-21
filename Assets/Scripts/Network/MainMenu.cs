using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] private NetworkRunner runner;
    [Header("Team")]
    [SerializeField] private TeamManager teamManager;

    [Header("Team Buttons")]
    [SerializeField] private Button team1Button;
    [SerializeField] private Button team2Button;

    [Header("Contadores")]
    [SerializeField] private TMP_Text team1Counter;
    [SerializeField] private TMP_Text team2Counter;

    [Header("Game")]
    //[SerializeField] private int gameSceneIndex = 0;

    [Header("Colores botones")]
    [SerializeField] private Color normalTeamColor = Color.white;
    [SerializeField] private Color disabledTeamColor = Color.gray;

    [Header("Sala")]
    [SerializeField] private TMP_InputField sessionNameInput;
    [Header("Menu")]
    [SerializeField] private NetworkMenu networkMenu;

    public static int PlayerTeam { get; private set; }

    private int selectedTeam = 0;

    private const string SessionName = "Match_1";

    private void Start()
    {
        if (teamManager == null)
        {
            teamManager =
                FindFirstObjectByType<TeamManager>();
        }

        selectedTeam = 0;
        PlayerTeam = 0;

        // Botones de equipo normales
        SetTeamButton(team1Button, true);
        SetTeamButton(team2Button, true);
    }
    private void UpdateTeamButtons()
    {
        if (teamManager == null)
            return;


        // =========================================
        // TODAVÍA NO ESTAMOS CONECTADOS
        // =========================================

        if (!teamManager.IsNetworkReady)
        {
            SetTeamButton(team1Button, false);
            SetTeamButton(team2Button, false);

            team1Counter.text = "0/2";
            team2Counter.text = "0/2";

            return;
        }


        // =========================================
        // CONTADORES
        // =========================================

        int team1Players = teamManager.GetTeam1Count();
        int team2Players = teamManager.GetTeam2Count();

        team1Counter.text = team1Players + "/2";
        team2Counter.text = team2Players + "/2";


        // =========================================
        // AMBOS EQUIPOS LLENOS
        // =========================================

        // Esto tiene que estar ANTES de
        // comprobar si este jugador ya eligió equipo.

        if (teamManager.AreAllTeamsFull())
        {
            Debug.Log("Los dos equipos están completos. " + "Pasando a selección de niveles.");

            networkMenu.ShowLevelSelector();

            return;
        }


        // =========================================
        // SI YA ELEGÍ EQUIPO
        // =========================================

        if (PlayerTeam != 0)
        {
            SetTeamButton(team1Button, false);
            SetTeamButton(team2Button, false);

            return;
        }


        // =========================================
        // EQUIPO 1
        // =========================================

        if (teamManager.IsTeamFull(1))
            SetTeamButton(team1Button, false);
        else
            SetTeamButton(team1Button, true);


        // =========================================
        // EQUIPO 2
        // =========================================

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


    public void SelectTeam1()
    {
        SelectTeam(1);
    }


    public void SelectTeam2()
    {
        SelectTeam(2);
    }


    private void SelectTeam(int team)
    {
        if (selectedTeam != 0)
        {
            Debug.LogWarning("Ya elegiste un equipo.");
            return;
        }

        if (teamManager == null)
        {
            Debug.LogWarning("TeamManager no está asignado.");
            return;
        }

        // Le pedimos al TeamManager que nos agregue al equipo
        teamManager.SelectTeam(team);

        selectedTeam = team;
        PlayerTeam = team;

        Debug.Log("Elegiste Equipo " + team);

        // Los dos botones quedan deshabilitados
        SetTeamButton(team1Button, false);
        SetTeamButton(team2Button, false);
    }

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

        Debug.Log("El Host está listo para cargar la escena Game.");
    }

}