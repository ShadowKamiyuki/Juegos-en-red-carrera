using Fusion;
using UnityEngine;

public class FinishLine : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Object.HasStateAuthority)
            return;

        Player player = other.GetComponent<Player>();

        if (player == null)
            return;

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
            return;

        gameManager.PlayerReachedFinish(player);
    }
}
