using Fusion;
using UnityEngine;

public class DashPowerUp : NetworkBehaviour
{
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float lifeTime = 20f;

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

        rb.linearVelocity = Vector2.down * fallSpeed;

        lifeTimer -= Runner.DeltaTime;

        if (lifeTimer <= 0f)
        {
            Runner.Despawn(Object);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        Player player = collision.GetComponent<Player>();

        if (player == null)
        {
            player = collision.GetComponentInParent<Player>();
        }

        if (player == null)
            return;

        player.ActivateDash();

        Runner.Despawn(Object);
    }
}