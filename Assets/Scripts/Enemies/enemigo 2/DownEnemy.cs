using Fusion;
using UnityEngine;

public class DownEnemy : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Despawn")]
    [SerializeField] private float lifeTime = 10f;

    private Rigidbody2D rb;
    private float lifeTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Spawned()
    {
        lifeTimer = lifeTime;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        // Siempre hacia abajo
        rb.linearVelocity = Vector2.down * moveSpeed;

        // Evitamos que se quede existiendo para siempre
        lifeTimer -= Runner.DeltaTime;

        if (lifeTimer <= 0)
        {
            Runner.Despawn(Object);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.Kill();
        }
    }
}