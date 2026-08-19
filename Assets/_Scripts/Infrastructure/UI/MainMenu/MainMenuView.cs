using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup optionsGroup;

    public event Action OnStartClicked;
    public event Action OnOptionsClicked;
    public event Action OnQuitClicked;

    private void Awake()
    {
        startButton.onClick.AddListener(() => OnStartClicked?.Invoke());
        optionsButton.onClick.AddListener(() => OnOptionsClicked?.Invoke());
        quitButton.onClick.AddListener(() => OnQuitClicked?.Invoke());
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
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
}
