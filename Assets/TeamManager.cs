
using Fusion;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{
    // =========================================================
    // SLOTS DE LOS EQUIPOS
    // =========================================================

    [Networked]
    private PlayerRef Team1Slot1 { get; set; }

    [Networked]
    private PlayerRef Team1Slot2 { get; set; }

    [Networked]
    private PlayerRef Team2Slot1 { get; set; }

    [Networked]
    private PlayerRef Team2Slot2 { get; set; }


    // =========================================================
    // ESTADO
    // =========================================================

    [Networked]
    private NetworkBool AllTeamsFull { get; set; }

    public bool IsNetworkReady { get; private set; }


    // =========================================================
    // CHECKS
    // =========================================================

    [Header("Equipo 1")]
    [SerializeField] private GameObject team1Check1;
    [SerializeField] private GameObject team1Check2;

    [Header("Equipo 2")]
    [SerializeField] private GameObject team2Check1;
    [SerializeField] private GameObject team2Check2;


    // =========================================================
    // PANEL DE EQUIPOS COMPLETOS
    // =========================================================

    [Header("Panel equipos completos")]
    [SerializeField] private GameObject teamsFullPanel;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        SetAllChecks(false);

        if (teamsFullPanel != null)
            teamsFullPanel.SetActive(false);
    }


    // =========================================================
    // SPAWNED
    // =========================================================

    public override void Spawned()
    {
        IsNetworkReady = true;

        Debug.Log(
            "TeamManager Spawned. StateAuthority = " +
            HasStateAuthority
        );

        UpdateVisuals();

        if (teamsFullPanel != null)
            teamsFullPanel.SetActive(AllTeamsFull);
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

        UpdateVisuals();

        if (teamsFullPanel != null)
            teamsFullPanel.SetActive(AllTeamsFull);
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
                "El Equipo " + team + " está lleno."
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
            "Solicitud de equipo: " +
            player +
            " -> Equipo " +
            team
        );

        // Evitar que esté en dos equipos.
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
                    " -> Equipo 1 Slot 1"
                );
            }
            else if (Team1Slot2 == PlayerRef.None)
            {
                Team1Slot2 = player;

                Debug.Log(
                    player +
                    " -> Equipo 1 Slot 2"
                );
            }
            else
            {
                Debug.Log(
                    "Equipo 1 lleno."
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
                    " -> Equipo 2 Slot 1"
                );
            }
            else if (Team2Slot2 == PlayerRef.None)
            {
                Team2Slot2 = player;

                Debug.Log(
                    player +
                    " -> Equipo 2 Slot 2"
                );
            }
            else
            {
                Debug.Log(
                    "Equipo 2 lleno."
                );

                return;
            }
        }


        // Comprobar equipos.
        CheckIfTeamsAreFull();
    }


    // =========================================================
    // ELIMINAR JUGADOR DE CUALQUIER EQUIPO
    // =========================================================

    private void RemovePlayer(PlayerRef player)
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
            " desconectado."
        );
    }


    // =========================================================
    // COMPROBAR SI LOS DOS EQUIPOS ESTÁN LLENOS
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
                "================================"
            );

            Debug.Log(
                "TODOS LOS EQUIPOS ESTÁN COMPLETOS"
            );

            Debug.Log(
                "================================"
            );
        }
    }


    // =========================================================
    // SABER SI UN EQUIPO ESTÁ LLENO
    // =========================================================

    public bool IsTeamFull(int team)
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
    // ACTUALIZAR CHECKS
    // =========================================================

    private void UpdateVisuals()
    {
        if (!IsNetworkReady)
            return;


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
    // APAGAR CHECKS
    // =========================================================

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
}
