using Fusion;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [Header("Global Services")]
    [RequiredField, SerializeField] private CustomUpdateManager updateManager;
    [RequiredField, SerializeField] private GameManager gameManager;
    [RequiredField, SerializeField] private SceneLoaderService sceneLoaderService;
    [RequiredField, SerializeField] private AudioManager audioManager;
    [RequiredField, SerializeField] private NetworkManager networkManager;

    [Header("Dependencies")]
    [RequiredField, SerializeField] private GameStateAudioController audioController;
    [RequiredField, SerializeField] private LoadingView loadingView;
    [RequiredField, SerializeField] private FadeController fadeController;
    [RequiredField, SerializeField] private NetworkRunner networkRunner;

    private IInputService inputService;

    // Entry point
    private void Awake()
    {
        inputService = new InputService();
        ServiceLocator.Register(inputService);

        RegisterService<ISceneLoader>(sceneLoaderService);
        RegisterService<IUpdateService>(updateManager);
        RegisterService<IAppStateMachine>(gameManager);
        RegisterService<IAudioService>(audioManager);
        RegisterService<INetworkService>(networkManager);

        StateFactory stateFactory = new StateFactory(gameManager, sceneLoaderService, loadingView, fadeController);

        gameManager.RegisterStates(stateFactory.Create());

        gameManager.Init(inputService);
        audioController.Init(gameManager, audioManager);
        networkManager.Init(networkRunner, inputService);
    }

    // Exit point
    private void OnDestroy()
    {
        (inputService as InputService)?.Dispose();
        ServiceLocator.Clear();
    }

    private void RegisterService<T>(T service)
    {
        if (service == null)
        {
            Debug.LogError($"Servicio {typeof(T).Name} es null");
            return;
        }

        ServiceLocator.Register(service);
    }
}
