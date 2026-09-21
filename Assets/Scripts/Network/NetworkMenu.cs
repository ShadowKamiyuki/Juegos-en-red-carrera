using Fusion;
using TMPro;
using UnityEngine;

public class NetworkMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField input;

    [SerializeField] private CanvasGroup networkMenu;
    [SerializeField] private CanvasGroup selectLevels;
    [SerializeField] private CanvasGroup selectTeams;
    [SerializeField] private NetworkRunner runner;

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
        // Ocultamos selección de equipos
        selectTeams.alpha = 0;
        selectTeams.interactable = false;
        selectTeams.blocksRaycasts = false;


        // Mostramos selección de niveles
        selectLevels.alpha = 1;

        if (runner != null && runner.IsSceneAuthority)
        {
            // HOST
            selectLevels.interactable = true;
            selectLevels.blocksRaycasts = true;

            Debug.Log("Host: puede seleccionar nivel.");
        }
        else
        {
            // CLIENTE
            selectLevels.interactable = false;
            selectLevels.blocksRaycasts = false;

            Debug.Log("Cliente: no puede seleccionar nivel.");
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}