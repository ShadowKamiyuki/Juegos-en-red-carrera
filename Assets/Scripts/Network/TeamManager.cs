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


    public override void Spawned()
    {
        IsNetworkReady = true;
        Debug.Log("TEAM MANAGER SPAWNED CORRECTAMENTE");
    }


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        IsNetworkReady = false;
    }


    // =========================================
    // SELECCIONAR EQUIPO
    // =========================================

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

        if (IsTeamFull(team))
        {
            Debug.Log(
                "El Equipo " + team + " está lleno."
            );

            return;
        }

        RequestTeamRpc(team, Runner.LocalPlayer);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RequestTeamRpc(int team, PlayerRef player)
    {
        Debug.Log(
            "RECIBÍ SOLICITUD DE EQUIPO " +
            team +
            " DEL JUGADOR " +
            player
        );

        // Comprobar ANTES de quitarlo de su equipo actual
        if (IsTeamFull(team))
        {
            Debug.Log("Equipo " + team + " está lleno.");
            return;
        }

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
        }
        else
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
        }

        CheckIfTeamsAreFull();
    }


    // =========================================
    // DESCONECTAR JUGADOR
    // =========================================

    public void RemoveDisconnectedPlayer(PlayerRef player)
    {
        if (Runner == null)
            return;

        if (!Runner.IsServer)
            return;

        Debug.Log(
            "Jugador " + player +
            " se desconectó."
        );

        // Liberamos su lugar
        RemovePlayer(player);

        // Volvemos a comprobar si los equipos
        // siguen completos
        CheckIfTeamsAreFull();

        Debug.Log(
            "Se liberó el lugar del jugador " +
            player
        );
    }


    // =========================================
    // SACAR JUGADOR DE LOS EQUIPOS
    // =========================================

    private void RemovePlayer(PlayerRef player)
    {
        if (Team1Slot1 == player)
        {
            Team1Slot1 = PlayerRef.None;
        }

        if (Team1Slot2 == player)
        {
            Team1Slot2 = PlayerRef.None;
        }

        if (Team2Slot1 == player)
        {
            Team2Slot1 = PlayerRef.None;
        }

        if (Team2Slot2 == player)
        {
            Team2Slot2 = PlayerRef.None;
        }
    }


    // =========================================
    // CONTADORES
    // =========================================

    public int GetTeam1Count()
    {
        int cantidad = 0;

        if (Team1Slot1 != PlayerRef.None)
        {
            cantidad++;
        }

        if (Team1Slot2 != PlayerRef.None)
        {
            cantidad++;
        }

        return cantidad;
    }

    public int GetTeam2Count()
    {
        int cantidad = 0;

        if (Team2Slot1 != PlayerRef.None)
        {
            cantidad++;
        }

        if (Team2Slot2 != PlayerRef.None)
        {
            cantidad++;
        }


        return cantidad;
    }


    // =========================================
    // EQUIPOS LLENOS
    // =========================================
    public bool AreAllTeamsFull()
    {
        return AllTeamsFull;
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
            Debug.Log("=================================");
            Debug.Log("¡LOS DOS EQUIPOS ESTÁN COMPLETOS!");
            Debug.Log("=================================");

            Debug.Log(
                "Jugador " + Team1Slot1 +
                " eligió EQUIPO 1"
            );

            Debug.Log(
                "Jugador " + Team1Slot2 +
                " eligió EQUIPO 1"
            );

            Debug.Log(
                "Jugador " + Team2Slot1 +
                " eligió EQUIPO 2"
            );

            Debug.Log(
                "Jugador " + Team2Slot2 +
                " eligió EQUIPO 2"
            );
        }
        else
        {
            Debug.Log("Todavía hay lugares disponibles.");
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