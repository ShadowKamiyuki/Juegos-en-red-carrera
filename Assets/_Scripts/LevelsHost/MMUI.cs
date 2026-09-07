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
        mainMenuPanel.SetActive(true);
        levelsPanel.SetActive(false);
    }

    private void Update()
    {
        if (teamManager == null)
            return;

        if (levelsShown)
            return;

        bool team1Full = teamManager.IsTeamFull(1);
        bool team2Full = teamManager.IsTeamFull(2);

        if (team1Full && team2Full)
        {
            ShowLevels();
        }
    }

    private void ShowLevels()
    {
        levelsShown = true;

        mainMenuPanel.SetActive(false);
        levelsPanel.SetActive(true);

        Debug.Log("Ambos equipos están llenos pibe.");
    }

    public void ShowMainMenu()
    {
        levelsShown = false;

        mainMenuPanel.SetActive(true);
        levelsPanel.SetActive(false);
    }
}