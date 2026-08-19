public interface ILoadingView
{
    void Show();
    void Hide();
    void ResetProgress();
    void SetProgress(float progress);
}
