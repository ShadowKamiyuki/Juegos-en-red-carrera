using Fusion;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{
    // =========================================================
    // EQUIPO 1
    // =========================================================

    [Networked]
    private PlayerRef Team1Slot1 { get; set; }

    [Networked]
    private PlayerRef Team1Slot2 { get; set; }


    // =========================================================
    // EQUIPO 2
    // =========================================================

    [Networked]
    private PlayerRef Team2Slot1 { get; set; }

    [Networked]
    private PlayerRef Team2Slot2 { get; set; }


    // =========================================================
    // TODOS LOS EQUIPOS COMPLETOS
    // =========================================================

    [Networked]
    private NetworkBool AllTeamsFull { get; set; }


    // =========================================================
    // ESTADO
    // =========================================================

    public bool IsNetworkReady { get; private set; }


    // =========================================================
    // TOGGLES / CHECKS
    // =========================================================

    [Header("Equipo 1")]
    [SerializeField] private GameObject team1Check1;
    [SerializeField] private GameObject team1Check2;

    [Header("Equipo 2")]
    [SerializeField] private GameObject team2Check1;
    [SerializeField] private GameObject team2Check2;


    // =========================================================
    // PANEL LEVELS
    // =========================================================

    [Header("Panel Levels")]
    [SerializeField] private GameObject levelsPanel;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        SetAllChecks(false);

        if (levelsPanel != null)
            levelsPanel.SetActive(false);
    }


    // =========================================================
    // SPAWNED
    // =========================================================

    public override void Spawned()
    {
        IsNetworkReady = true;

        Debug.Log(
            "TeamManager conectado a Fusion."
        );

        UpdateVisuals();
    }


    // =========================================================
    // DESPAWNED
    // =========================================================

    public override void Despawned(
        NetworkRunner runner,
        bool hasState)
    {
        IsNetworkReady = false;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!IsNetworkReady)
            return;


        // Actualizar checks
        UpdateVisuals();


        // Abrir/cerrar panel Levels
        if (levelsPanel != null)
        {
            levelsPanel.SetActive(
                AllTeamsFull
            );
        }
    }


    // =========================================================
    // SELECCIONAR EQUIPO
    // =========================================================

    public void SelectTeam(int team)
    {
        if (!IsNetworkReady)
        {
            Debug.LogWarning(
                "TeamManager todavía no está listo."
            );

            return;
        }


        if (team != 1 && team != 2)
        {
            Debug.LogWarning(
                "Equipo inválido: " + team
            );

            return;
        }


        if (IsTeamFull(team))
        {
            Debug.Log(
                "El Equipo " +
                team +
                " está lleno."
            );

            return;
        }


        RequestTeamRpc(team);
    }


    // =========================================================
    // RPC
    // =========================================================

    [Rpc(
        RpcSources.All,
        RpcTargets.StateAuthority
    )]
    private void RequestTeamRpc(
        int team,
        RpcInfo info = default)
    {
        PlayerRef player = info.Source;


        Debug.Log(
            "Jugador " +
            player +
            " solicita Equipo " +
            team
        );


        // Sacarlo de cualquier equipo anterior
        RemovePlayer(player);


        // =====================================================
        // EQUIPO 1
        // =====================================================

        if (team == 1)
        {
            if (Team1Slot1 == PlayerRef.None)
            {
                Team1Slot1 = player;

                Debug.Log(
                    player +
                    " -> Equipo 1 / Slot 1"
                );
            }
            else if (Team1Slot2 == PlayerRef.None)
            {
                Team1Slot2 = player;

                Debug.Log(
                    player +
                    " -> Equipo 1 / Slot 2"
                );
            }
            else
            {
                Debug.Log(
                    "Equipo 1 está lleno."
                );

                return;
            }
        }


        // =====================================================
        // EQUIPO 2
        // =====================================================

        else
        {
            if (Team2Slot1 == PlayerRef.None)
            {
                Team2Slot1 = player;

                Debug.Log(
                    player +
                    " -> Equipo 2 / Slot 1"
                );
            }
            else if (Team2Slot2 == PlayerRef.None)
            {
                Team2Slot2 = player;

                Debug.Log(
                    player +
                    " -> Equipo 2 / Slot 2"
                );
            }
            else
            {
                Debug.Log(
                    "Equipo 2 está lleno."
                );

                return;
            }
        }


        // Comprobar si ambos equipos están completos
        CheckIfTeamsAreFull();
    }


    // =========================================================
    // QUITAR JUGADOR
    // =========================================================

    private void RemovePlayer(
        PlayerRef player)
    {
        if (Team1Slot1 == player)
            Team1Slot1 = PlayerRef.None;

        if (Team1Slot2 == player)
            Team1Slot2 = PlayerRef.None;

        if (Team2Slot1 == player)
            Team2Slot1 = PlayerRef.None;

        if (Team2Slot2 == player)
            Team2Slot2 = PlayerRef.None;
    }


    // =========================================================
    // JUGADOR DESCONECTADO
    // =========================================================

    public void RemoveDisconnectedPlayer(
        PlayerRef player)
    {
        if (!IsNetworkReady)
            return;


        if (!HasStateAuthority)
            return;


        RemovePlayer(player);

        CheckIfTeamsAreFull();


        Debug.Log(
            "Jugador " +
            player +
            " salió. Slot liberado."
        );
    }


    // =========================================================
    // ACTUALIZAR CHECKS
    // =========================================================

    private void UpdateVisuals()
    {
        if (!IsNetworkReady)
            return;


        // -----------------------------------------------------
        // EQUIPO 1
        // -----------------------------------------------------

        if (team1Check1 != null)
        {
            team1Check1.SetActive(
                Team1Slot1 != PlayerRef.None
            );
        }


        if (team1Check2 != null)
        {
            team1Check2.SetActive(
                Team1Slot2 != PlayerRef.None
            );
        }


        // -----------------------------------------------------
        // EQUIPO 2
        // -----------------------------------------------------

        if (team2Check1 != null)
        {
            team2Check1.SetActive(
                Team2Slot1 != PlayerRef.None
            );
        }


        if (team2Check2 != null)
        {
            team2Check2.SetActive(
                Team2Slot2 != PlayerRef.None
            );
        }
    }


    // =========================================================
    // COMPROBAR EQUIPOS COMPLETOS
    // =========================================================

    private void CheckIfTeamsAreFull()
    {
        if (!IsNetworkReady)
            return;


        if (!HasStateAuthority)
            return;


        bool team1Full =
            Team1Slot1 != PlayerRef.None &&
            Team1Slot2 != PlayerRef.None;


        bool team2Full =
            Team2Slot1 != PlayerRef.None &&
            Team2Slot2 != PlayerRef.None;


        AllTeamsFull =
            team1Full &&
            team2Full;


        Debug.Log(
            "Equipo 1: " +
            (team1Full ? "2/2" : "NO COMPLETO")
        );


        Debug.Log(
            "Equipo 2: " +
            (team2Full ? "2/2" : "NO COMPLETO")
        );


        if (AllTeamsFull)
        {
            Debug.Log(
                "¡LOS DOS EQUIPOS ESTÁN COMPLETOS!"
            );
        }
    }


    // =========================================================
    // ¿EQUIPO LLENO?
    // =========================================================

    public bool IsTeamFull(
        int team)
    {
        if (!IsNetworkReady)
            return false;


        if (team == 1)
        {
            return
                Team1Slot1 != PlayerRef.None &&
                Team1Slot2 != PlayerRef.None;
        }


        if (team == 2)
        {
            return
                Team2Slot1 != PlayerRef.None &&
                Team2Slot2 != PlayerRef.None;
        }


        return true;
    }


    // =========================================================
    // APAGAR TODOS LOS CHECKS
    // =========================================================

    private void SetAllChecks(
        bool state)
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
}