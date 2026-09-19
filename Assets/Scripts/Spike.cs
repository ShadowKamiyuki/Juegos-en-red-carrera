using Fusion;
using UnityEngine;

public class Spike : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // aplicar daño al jugador
        }
    }
}
