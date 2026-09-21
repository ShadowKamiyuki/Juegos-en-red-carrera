using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NetworkMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField input;

    [SerializeField] private CanvasGroup networkMenu;
    [SerializeField] private CanvasGroup selectLevels;
    [SerializeField] private CanvasGroup selectTeams;
    [SerializeField] private NetworkRunner runner;

    [Header("Botones de niveles")]
    [SerializeField] private Button[] levelButtons;

    private void OnEnable()
    {
        NetworkManager.OnConnectedToGame += ConnectedToGame;
    }


    private void OnDisable()
    {
        NetworkManager.OnConnectedToGame -= ConnectedToGame;
    }


    private void ConnectedToGame()
    {
        Hide();
        ShowTeams();
    }


    public void CreateGame()
    {
        if (string.IsNullOrEmpty(input.text))
        {
            Debug.LogWarning("Escribí un nombre para la sala.");
            return;
        }

        networkManager.StartGameHost(input.text);
    }


    public void JoinGame()
    {
        if (string.IsNullOrEmpty(input.text))
        {
            Debug.LogWarning("Escribí un nombre para la sala.");
            return;
        }

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


    public void ShowTeams()
    {
        selectTeams.alpha = 1;
        selectTeams.interactable = true;
        selectTeams.blocksRaycasts = true;
    }


    public void ShowLevelSelector()
    {
        // =========================================
        // OCULTAR EQUIPOS
        // =========================================

        selectTeams.alpha = 0;
        selectTeams.interactable = false;
        selectTeams.blocksRaycasts = false;


        // =========================================
        // MOSTRAR NIVELES
        // =========================================

        selectLevels.alpha = 1;
        selectLevels.interactable = true;
        selectLevels.blocksRaycasts = true;


        // =========================================
        // HOST O CLIENTE
        // =========================================

        bool isHost = runner != null && runner.IsServer;

        foreach (Button button in levelButtons)
        {
            if (button == null)
                continue;

            button.interactable = isHost;
        }


        if (isHost)
        {
            Debug.Log("SOY HOST -> puedo elegir nivel");
        }
        else
        {
            Debug.Log("SOY CLIENTE -> niveles bloqueados");
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}