using UnityEngine;

public class Setup : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject musicSource;
    [SerializeField] private GameObject sfxPool;

    private void Awake()
    {
        ServiceLocator.Register<IAudioService>(audioManager);

        DontDestroyOnLoad(audioManager);
        DontDestroyOnLoad(musicSource);
        DontDestroyOnLoad(sfxPool);
    }

    // cannot be used, it clears audio ervice when changing scenes
    //private void OnDestroy()
    //{
    //    ServiceLocator.Clear();
    //}
}
