using Fusion;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{
    // ==================================================
    // EQUIPO 1
    // ==================================================

    [Networked]
    private PlayerRef Team1Slot1 { get; set; }

    [Networked]
    private PlayerRef Team1Slot2 { get; set; }

    public bool IsNetworkReady { get; private set; }

    // ==================================================
    // EQUIPO 2
    // ==================================================

    [Networked]
    private PlayerRef Team2Slot1 { get; set; }

    [Networked]
    private PlayerRef Team2Slot2 { get; set; }

    // ==================================================
    // CHECKS
    // ==================================================

    [Header("Equipo 1")]
    [SerializeField] private GameObject team1Check1;
    [SerializeField] private GameObject team1Check2;

    [Header("Equipo 2")]
    [SerializeField] private GameObject team2Check1;
    [SerializeField] private GameObject team2Check2;

    // ==================================================
    // SPAWN
    // ==================================================

    public override void Spawned()
    {
        IsNetworkReady = true;

        UpdateVisuals();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        IsNetworkReady = false;
    }
    // ==================================================
    // UPDATE
    // ==================================================

    private void Update()
    {
        UpdateVisuals();
    }

    // ==================================================
    // SELECCIONAR EQUIPO
    // ==================================================

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

    // ==================================================
    // RPC
    // ==================================================

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
    }

    // ==================================================
    // QUITAR JUGADOR
    // ==================================================

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

    // ==================================================
    // JUGADOR DESCONECTADO
    // ==================================================

    public void RemoveDisconnectedPlayer(
        PlayerRef player)
    {
        if (Runner == null)
            return;

        if (!Runner.IsServer)
            return;

        RemovePlayer(player);

        Debug.Log(
            "Jugador " +
            player +
            " salió. Se liberó su lugar."
        );
    }

    // ==================================================
    // ACTUALIZAR CHECKS
    // ==================================================

    private void UpdateVisuals()
    {
        if (Runner == null)
            return;

        // ==========================================
        // EQUIPO 1
        // ==========================================

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

        // ==========================================
        // EQUIPO 2
        // ==========================================

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

    // ==================================================
    // EQUIPO LLENO
    // ==================================================

    public bool IsTeamFull(int team)
    {
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