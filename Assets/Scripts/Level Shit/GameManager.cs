using Fusion;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [Networked]public int WinningTeam { get; set; }

    private bool matchStarted;
    private bool networkReady;

    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button teamSelectionButton;
    
    private bool ShowPanel;


    public override void Spawned()
    {
        networkReady = true;

        if (Object.HasStateAuthority)
        {
            WinningTeam = 0;
            matchStarted = false;
        }

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        ShowPanel = false;
    }


    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (WinningTeam != 0)
            return;

        if (!AreAllPlayersReady())
            return;

        matchStarted = true;

        CheckWinner();
    }


    private void Update()
    {
        if (!networkReady)
            return;

        if (WinningTeam == 0)
            return;

        if (ShowPanel)
            return;

        ShowEndGamePanel();
    }


    private bool AreAllPlayersReady()
    {
        bool foundPlayer = false;

        foreach (PlayerRef playerRef in Runner.ActivePlayers)
        {
            foundPlayer = true;

            NetworkObject playerObject =
                Runner.GetPlayerObject(playerRef);

            if (playerObject == null)
                return false;

            Player player =
                playerObject.GetComponent<Player>();

            if (player == null)
                return false;

            if (player.Team == 0)
                return false;
        }

        return foundPlayer;
    }


    private void CheckWinner()
    {
        bool team1Alive = false;
        bool team2Alive = false;


        foreach (PlayerRef playerRef in Runner.ActivePlayers)
        {
            NetworkObject playerObject =
                Runner.GetPlayerObject(playerRef);

            if (playerObject == null)
                continue;

            Player player =
                playerObject.GetComponent<Player>();

            if (player == null)
                continue;

            if (!player.IsAlive)
                continue;


            if (player.Team == 1)
            {
                team1Alive = true;
            }
            else if (player.Team == 2)
            {
                team2Alive = true;
            }
        }


        if (!team1Alive && team2Alive)
        {
            WinningTeam = 2;

            Debug.Log("¡Gana el equipo 2!");

        }
        else if (team1Alive && !team2Alive)
        {
            WinningTeam = 1;

            Debug.Log("¡Gana el equipo 1!");

        }
        else if (!team1Alive && !team2Alive)
        {
            WinningTeam = 3;

            Debug.Log("¡Empate!");

        }
    }


    private void ShowEndGamePanel()
    {
        ShowPanel = true;

        if (endGamePanel == null)
            return;

        endGamePanel.SetActive(true);


        if (resultText != null)
        {
            if (WinningTeam == 1)
            {
                resultText.text = "¡GANA EL EQUIPO 1!";
            }
            else if (WinningTeam == 2)
            {
                resultText.text = "¡GANA EL EQUIPO 2!";
            }
            else if (WinningTeam == 3)
            {
                resultText.text = "¡EMPATE!";
            }
        }
    }


    // =========================================
    // REINICIAR NIVEL
    // =========================================

    public void RestartLevel()
    {
        RestartLevelRpc();
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RestartLevelRpc()
    {
        Debug.Log("Reiniciando nivel.");

        Runner.LoadScene(SceneRef.FromIndex(1));
    }


    // =========================================
    // VOLVER A SELECCIÓN DE EQUIPO
    // =========================================

    public void ReturnToTeamSelection()
    {
        ReturnToTeamSelectionRpc();
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void ReturnToTeamSelectionRpc()
    {
        TeamManager teamManager = FindFirstObjectByType<TeamManager>();

        if (teamManager != null)
        {
            teamManager.ResetTeams();
        }

        Debug.Log("Volviendo a selección de equipos.");

        Runner.LoadScene(SceneRef.FromIndex(0));
    }


    public bool HasMatchEnded()
    {
        return WinningTeam != 0;
    }
}