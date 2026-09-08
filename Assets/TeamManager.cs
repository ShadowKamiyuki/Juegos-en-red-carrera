using Fusion;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{


    [Networked]
    private PlayerRef Team1Slot1 { get; set; }

    [Networked]
    private PlayerRef Team1Slot2 { get; set; }


    [Networked]
    private PlayerRef Team2Slot1 { get; set; }

    [Networked]
    private PlayerRef Team2Slot2 { get; set; }


    [Networked]
    private NetworkBool AllTeamsFull { get; set; }

    public bool IsNetworkReady { get; private set; }


    [Header("Panel cuando todos los equipos están llenos")]
    [SerializeField] private GameObject teamsFullPanel;


    [Header("Equipo 1")]
    [SerializeField] private GameObject team1Check1;
    [SerializeField] private GameObject team1Check2;


    [Header("Equipo 2")]
    [SerializeField] private GameObject team2Check1;
    [SerializeField] private GameObject team2Check2;



    private void Awake()
    {
        // El panel SIEMPRE comienza oculto.
        if (teamsFullPanel != null)
        {
            teamsFullPanel.SetActive(false);
        }

        // Los checks también comienzan apagados.
        SetAllChecks(false);
    }


    public override void Spawned()
    {
        IsNetworkReady = true;

        // Todos empiezan con el panel apagado.
        if (teamsFullPanel != null)
        {
            teamsFullPanel.SetActive(false);
        }

        UpdateVisuals();
    }


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        IsNetworkReady = false;
    }


    private void Update()
    {
        if (!IsNetworkReady)
            return;

        UpdateVisuals();

        if (teamsFullPanel != null)
        {
            teamsFullPanel.SetActive(AllTeamsFull);
        }
    }


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
            "Solicitud de equipo recibida de " +
            player +
            " -> Equipo " +
            team
        );

        // Primero sacar al jugador de cualquier equipo.
        RemovePlayer(player);


        if (team == 1)
        {
            if (Team1Slot1 == PlayerRef.None)
            {
                Team1Slot1 = player;

                Debug.Log(
                    player +
                    " entró al Equipo 1 - Slot 1"
                );
            }
            else if (Team1Slot2 == PlayerRef.None)
            {
                Team1Slot2 = player;

                Debug.Log(
                    player +
                    " entró al Equipo 1 - Slot 2"
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


        else
        {
            if (Team2Slot1 == PlayerRef.None)
            {
                Team2Slot1 = player;

                Debug.Log(
                    player +
                    " entró al Equipo 2 - Slot 1"
                );
            }
            else if (Team2Slot2 == PlayerRef.None)
            {
                Team2Slot2 = player;

                Debug.Log(
                    player +
                    " entró al Equipo 2 - Slot 2"
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


        // Comprobar si ambos equipos están completos.
        CheckIfTeamsAreFull();
    }


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


    public void RemoveDisconnectedPlayer(
        PlayerRef player)
    {
        if (!IsNetworkReady)
            return;

        if (!Runner.IsServer)
            return;

        RemovePlayer(player);

        CheckIfTeamsAreFull();

        Debug.Log(
            "Jugador " +
            player +
            " salió. Se liberó su lugar."
        );
    }


    private void UpdateVisuals()
    {
        if (!IsNetworkReady)
            return;


        // EQUIPO 1

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


        // EQUIPO 2

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


    private void CheckIfTeamsAreFull()
    {
        if (!IsNetworkReady)
            return;

        if (!Runner.IsServer)
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
            (team1Full ? "LLENO" : "NO LLENO")
        );

        Debug.Log(
            "Equipo 2: " +
            (team2Full ? "LLENO" : "NO LLENO")
        );


        if (AllTeamsFull)
        {
            Debug.Log(
                "================================"
            );

            Debug.Log(
                "LOS DOS EQUIPOS ESTÁN COMPLETOS"
            );

            Debug.Log(
                "================================"
            );
        }
    }


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