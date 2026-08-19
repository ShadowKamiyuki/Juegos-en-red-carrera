using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour, IFadeService
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        SetAlpha(0f);
    }

    public Task FadeToBlackAsync()
    {
        return FadeAsync(0f, 1f);
    }

    public Task FadeFromBlackAsync()
    {
        return FadeAsync(1f, 0f);
    }

    private async Task FadeAsync(float from, float to)
    {
        float elapsed = 0f;
        SetAlpha(from);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            SetAlpha(Mathf.Lerp(from, to, t));
            await Task.Yield();
        }

        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}
