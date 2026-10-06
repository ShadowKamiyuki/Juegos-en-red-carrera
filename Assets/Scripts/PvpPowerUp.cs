using Fusion;
using UnityEngine;

public class PvpPowerUp : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>()?.ActivatePowerUp();
        }
    }
}
