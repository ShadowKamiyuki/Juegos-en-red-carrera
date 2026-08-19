using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : MonoBehaviour, ILoadingView
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressBar;

    private void Awake()
    {
        Hide();
        ResetProgress();
    }


    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void SetProgress(float value)
    {
        progressBar.fillAmount = Mathf.Clamp01(value);
    }

    public void ResetProgress()
    {
        SetProgress(0f);
    }
}