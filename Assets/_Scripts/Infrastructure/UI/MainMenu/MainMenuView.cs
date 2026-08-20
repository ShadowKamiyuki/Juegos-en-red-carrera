using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [Header("Main menu")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Network menu")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    [Header("Screens")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup optionsGroup;
    [SerializeField] private CanvasGroup NetworkGroup;

    public event Action OnStartClicked;
    public event Action OnOptionsClicked;
    public event Action OnQuitClicked;
    public event Action OnHostClicked;
    public event Action OnJoinClicked;

    private void Awake()
    {
        startButton.onClick.AddListener(() => OnStartClicked?.Invoke());
        optionsButton.onClick.AddListener(() => OnOptionsClicked?.Invoke());
        quitButton.onClick.AddListener(() => OnQuitClicked?.Invoke());
        hostButton.onClick.AddListener(() => OnHostClicked?.Invoke());
        joinButton.onClick.AddListener(() => OnJoinClicked?.Invoke());
    }

    public void ShowMainMenu()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideMainMenu()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowOptions()
    {
        optionsGroup.alpha = 1;
        optionsGroup.interactable = true;
        optionsGroup.blocksRaycasts = true;
    }

    public void HideOptions()
    {
        optionsGroup.alpha = 0;
        optionsGroup.interactable = false;
        optionsGroup.blocksRaycasts = false;
    }

    public void ShowNetwork()
    {
        NetworkGroup.alpha = 1;
        NetworkGroup.interactable = true;
        NetworkGroup.blocksRaycasts = true;
    }

    public void HideNetwork()
    {
        NetworkGroup.alpha = 0;
        NetworkGroup.interactable = false;
        NetworkGroup.blocksRaycasts = false;
    }
}
