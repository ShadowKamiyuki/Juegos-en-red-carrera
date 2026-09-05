using Fusion;
using UnityEngine;
using UnityEngine.UI;
// Codigo puramente pensado para "mejorar" la UI (?

public class HostOnlyLevelMenu : NetworkBehaviour
{
    [SerializeField] private Button[] levelButtons;

    public override void Spawned()
    {
        bool isHost = Runner.IsSceneAuthority;

        foreach (Button button in levelButtons)
        {
            button.interactable = isHost;
        }
    }
}