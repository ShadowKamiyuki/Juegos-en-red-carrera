using UnityEngine;

public class Setup : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        ServiceLocator.Register<IAudioService>(audioManager);
    }

    private void OnDestroy()
    {
        ServiceLocator.Clear();
    }

}
