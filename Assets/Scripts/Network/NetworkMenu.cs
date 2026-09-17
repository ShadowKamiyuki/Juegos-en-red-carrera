using TMPro;
using UnityEngine;

public class NetworkMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private CanvasGroup networkMenu;
    [SerializeField] private CanvasGroup selectLevels;
    [SerializeField] private CanvasGroup selectTeams;

    public void CreateGame()
    {
        networkManager.StartGameHost(input.text);
    }

    public void JoinGame()
    {
        networkManager.StartGameClient(input.text);
    }

    public void Hide()
    {
        networkMenu.alpha = 0;
        networkMenu.interactable = false;
        networkMenu.blocksRaycasts = false;
    }

    public void Show()
    {
        networkMenu.alpha = 1;
        networkMenu.interactable = true;
        networkMenu.blocksRaycasts = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ShowTeems()
    {
        selectTeams.alpha = 1;
        selectTeams.interactable = true;
        selectTeams.blocksRaycasts = true;
    }
    public void ShowLevelSelector()
    {
        selectLevels.alpha = 1;
        selectLevels.interactable = true;
        selectLevels.blocksRaycasts = true;
    }
}
