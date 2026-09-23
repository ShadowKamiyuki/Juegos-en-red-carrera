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

    [Header("Selector de niveles")]
    [SerializeField] private TMP_Text waitingHostText;

    [Header("Botones de niveles")]
    [SerializeField] private Button[] levelButtons;
    [Header("Error de conexión")]
    [SerializeField] private CanvasGroup connectionError;

    private void OnEnable()
    {
        NetworkManager.OnConnectedToGame += ConnectedToGame;
        NetworkManager.OnConnectionFailed += ConnectionFailed;
    }

    private void OnDisable()
    {
        NetworkManager.OnConnectedToGame -= ConnectedToGame;
        NetworkManager.OnConnectionFailed -= ConnectionFailed;
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
    private void ConnectionFailed()
    {
        Debug.Log("No se pudo conectar a la partida.");

        Show();

        connectionError.alpha = 1;
        connectionError.interactable = true;
        connectionError.blocksRaycasts = true;
    }
    public void CloseConnectionError()
    {
        connectionError.alpha = 0;
        connectionError.interactable = false;
        connectionError.blocksRaycasts = false;

        Show();
        input.text = "";
        input.Select();
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
        // Mostrar equipos
        selectTeams.alpha = 1;
        selectTeams.interactable = true;
        selectTeams.blocksRaycasts = true;

        // Ocultar niveles
        selectLevels.alpha = 0;
        selectLevels.interactable = false;
        selectLevels.blocksRaycasts = false;
    }


    public void ShowLevelSelector()
    {
        Debug.Log("CAMBIANDO AL PANEL DE NIVELES");

        // Ocultar equipos
        selectTeams.alpha = 0;
        selectTeams.interactable = false;
        selectTeams.blocksRaycasts = false;

        // Mostrar niveles
        selectLevels.alpha = 1;
        selectLevels.interactable = true;
        selectLevels.blocksRaycasts = true;

        // Ver si soy Host
        bool isHost = runner != null && runner.IsServer;

        foreach (Button button in levelButtons)
        {
            if (button == null)
                continue;

            button.interactable = isHost;
        }

        if (waitingHostText != null)
        {
            waitingHostText.gameObject.SetActive(!isHost);
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