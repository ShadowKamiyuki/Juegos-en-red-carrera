using UnityEngine;

public class GameplayInstaller : MonoBehaviour, IGameplayInstaller
{
    [RequiredField, SerializeField] private Player player;
    [RequiredField, SerializeField] private CameraFollow playerCamera;
    [RequiredField, SerializeField] private PlayerController playerController;

    private void Awake()
    {
        ServiceLocator.Register<IGameplayInstaller>(this);
    }

    private void OnDestroy()
    {
        if (ServiceLocator.Exists<IGameplayInstaller>())
            ServiceLocator.Unregister<IGameplayInstaller>();
    }

    public void Init()
    {
        IInputService input = ServiceLocator.Get<IInputService>();
        playerController.Construct(input);

        playerCamera.SetTarget(player);
    }

    public void Dispose()
    {

    }
}
