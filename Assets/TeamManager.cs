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

    [Header("Equipo 1")]
    [SerializeField] private GameObject team1Check1;
    [SerializeField] private GameObject team1Check2;

    [Header("Equipo 2")]
    [SerializeField] private GameObject team2Check1;
    [SerializeField] private GameObject team2Check2;

    public override void Spawned()
    {
        UpdateVisuals();
    }

    private void Update()
    {
        UpdateVisuals();
    }
    public void SelectTeam(int team)
    {
        if (Runner == null)
        {
            Debug.LogWarning("TeamManager todavía no está conectado a Fusion.");
            return;
        }

        if (!Object.HasStateAuthority)
        {
            RequestTeamRpc(team);
            return;
        }

        RequestTeamRpc(team);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RequestTeamRpc(
        int team,
        RpcInfo info = default)
    {
        PlayerRef player = info.Source;

        // Primero sacamos al jugador de cualquier equipo
        RemovePlayer(player);

        if (team == 1)
        {
            if (Team1Slot1 == PlayerRef.None)
            {
                Team1Slot1 = player;

                Debug.Log(player + " entró al Equipo 1 - Slot 1");
            }
            else if (Team1Slot2 == PlayerRef.None)
            {
                Team1Slot2 = player;

                Debug.Log(player + " entró al Equipo 1 - Slot 2");
            }
            else
            {
                Debug.Log(player + " intentó entrar al Equipo 1, pero está lleno.");
            }
        }
        else if (team == 2)
        {
            if (Team2Slot1 == PlayerRef.None)
            {
                Team2Slot1 = player;

                Debug.Log(player + " entró al Equipo 2 - Slot 1");
            }
            else if (Team2Slot2 == PlayerRef.None)
            {
                Team2Slot2 = player;

                Debug.Log(player + " entró al Equipo 2 - Slot 2");
            }
            else
            {
                Debug.Log(player + " intentó entrar al Equipo 2, pero está lleno.");
            }
        }
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

    public void RemoveDisconnectedPlayer(PlayerRef player)
    {
        if (!Runner.IsServer)
            return;

        RemovePlayer(player);
    }

    private void UpdateVisuals()
    {
        if (Runner == null)
            return;

        if (team1Check1 != null)
            team1Check1.SetActive(Team1Slot1 != PlayerRef.None);

        if (team1Check2 != null)
            team1Check2.SetActive(Team1Slot2 != PlayerRef.None);

        if (team2Check1 != null)
            team2Check1.SetActive(Team2Slot1 != PlayerRef.None);

        if (team2Check2 != null)
            team2Check2.SetActive(Team2Slot2 != PlayerRef.None);
    }

    public bool IsTeamFull(int team)
    {
        if (team == 1)
        {
            return Team1Slot1 != PlayerRef.None &&
                   Team1Slot2 != PlayerRef.None;
        }

        if (team == 2)
        {
            return Team2Slot1 != PlayerRef.None &&
                   Team2Slot2 != PlayerRef.None;
        }

        return true;
    }
}