using Fusion;
using System;
using UnityEngine;

public class MMUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelsPanel;

    [Header("Team")]
    [SerializeField] private TeamManager teamManager;

    private bool levelsShown = false;


    private void Start()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (levelsPanel != null)
            levelsPanel.SetActive(false);
    }


    private void Update()
    {
        if (teamManager == null)
            return;

        if (!teamManager.IsNetworkReady)
            return;

        if (levelsShown)
            return;


        bool team1Full =
            teamManager.IsTeamFull(1);

        bool team2Full =
            teamManager.IsTeamFull(2);


        if (team1Full && team2Full)
        {
            ShowLevels();
        }
    }


    private void ShowLevels()
    {
        if (levelsShown)
            return;

        levelsShown = true;


        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);


        if (levelsPanel != null)
            levelsPanel.SetActive(true);


        Debug.Log(
            "================================"
        );

        Debug.Log(
            "AMBOS EQUIPOS ESTÁN LLENOS"
        );

        Debug.Log(
            "MOSTRANDO LEVELS PANEL"
        );

        Debug.Log(
            "================================"
        );
    }


    public void ShowMainMenu()
    {
        levelsShown = false;


        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (levelsPanel != null)
            levelsPanel.SetActive(false);
    }


    public void OnPlayerLeft(
        NetworkRunner runner,
        PlayerRef player)
    {
        TeamManager teamManager =
            FindFirstObjectByType<TeamManager>();

        if (teamManager != null && runner.IsServer)
        {
            teamManager.RemoveDisconnectedPlayer(player);
        }

        Debug.Log(
            "Jugador desconectado: " + player
        );
    }
}
