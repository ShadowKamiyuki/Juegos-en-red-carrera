using Fusion;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{

    [Networked]
    private PlayerRef Team1Slot1 { get; set; }

    [Networked]
    private PlayerRef Team1Slot2 { get; set; }

    public bool IsNetworkReady { get; private set; }

    [Header("Panel cuando todos los equipos están llenos")]
    [SerializeField] private GameObject teamsFullPanel;

    [Networked]
    private NetworkBool AllTeamsFull { get; set; }


    [Networked]
    private PlayerRef Team2Slot1 { get; set; }

    [Networked]
    private PlayerRef Team2Slot2 { get; set; }


    [Header("Equipo 1")]
    [SerializeField] private GameObject team1Check1;
    [SerializeField] private GameObject team1Check2;

    [Header("Equipo 2")]
    [SerializeField] private GameObject team2Check1;
    [SerializeField] private GameObject team2Check2;

    private void Awake()
    {
        if (teamsFullPanel != null)
        {
            teamsFullPanel.SetActive(false);
        }
    }
    public override void Spawned()
    {
        IsNetworkReady = true;

        // Siempre empieza oculto
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
        if (Runner == null)
        {
            Debug.LogWarning(
                "TeamManager todavía no está conectado a Fusion."
            );

            return;
        }

        if (team != 1 && team != 2)
        {
            Debug.LogWarning("Equipo inválido: " + team);
            return;
        }

        // Comprobar si el equipo está lleno
        if (IsTeamFull(team))
        {
            Debug.Log("El Equipo " + team + " está lleno.");
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

        // Sacarlo primero de cualquier equipo anterior
        RemovePlayer(player);

        if (team == 1)
        {
            if (Team1Slot1 == PlayerRef.None)
            {
                Team1Slot1 = player;

                Debug.Log(
                    player + " entró al Equipo 1 - Slot 1"
                );
            }
            else if (Team1Slot2 == PlayerRef.None)
            {
                Team1Slot2 = player;

                Debug.Log(
                    player + " entró al Equipo 1 - Slot 2"
                );
            }
            else
            {
                Debug.Log(
                    "Equipo 1 está lleno. " +
                    player + " no puede entrar."
                );
            }
        }
        else
        {
            if (Team2Slot1 == PlayerRef.None)
            {
                Team2Slot1 = player;

                Debug.Log(
                    player + " entró al Equipo 2 - Slot 1"
                );
            }
            else if (Team2Slot2 == PlayerRef.None)
            {
                Team2Slot2 = player;

                Debug.Log(
                    player + " entró al Equipo 2 - Slot 2"
                );
            }
            else
            {
                Debug.Log(
                    "Equipo 2 está lleno. " +
                    player + " no puede entrar."
                );
            }
        }
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
        if (Runner == null)
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
        if (Runner == null)
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

    private void CheckIfTeamsAreFull()
    {
        if (Runner == null)
            return;

        if (!Runner.IsServer)
            return;

        bool team1Full =
            Team1Slot1 != PlayerRef.None &&
            Team1Slot2 != PlayerRef.None;

        bool team2Full =
            Team2Slot1 != PlayerRef.None &&
            Team2Slot2 != PlayerRef.None;

        AllTeamsFull = team1Full && team2Full;

        if (AllTeamsFull)
        {
            Debug.Log("¡Los dos equipos están completos!");
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
}