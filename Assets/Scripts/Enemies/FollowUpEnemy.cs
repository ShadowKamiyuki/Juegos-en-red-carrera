using Fusion;
using System;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class FollowUpEnemy : NetworkBehaviour
{
    [Header("Target")]
    [SerializeField] private float detectionRadious = 5;
    [SerializeField] private LayerMask playerLayer;

    [Header("Enemy settings")]
    [SerializeField] private float initialHealth = 10f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private AudioDefinition attackSound;

    [Networked] public float Health { get; set; }

    private Rigidbody2D rb;
    private NetworkObject target;
    private IAudioService audioService;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        audioService = ServiceLocator.Get<IAudioService>();
    }

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
            return;

        Health = initialHealth;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        GetClosestPlayer();
        FollowPlayer();
    }

    private void GetClosestPlayer()
    {
        // necesitamos una estrategia para buscar al jugador correcto
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, detectionRadious, playerLayer);

        target = null;

        float closestDistance = Mathf.Infinity;

        foreach (Collider2D player in players)
        {
            NetworkObject networkObject = player.GetComponentInParent<NetworkObject>();

            if (networkObject == null)
                continue;

            float distance = Vector2.Distance(transform.position, networkObject.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                // usa network transform
                target = networkObject;
            }
        }
    }

    private void FollowPlayer()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // using Steearing behaviour seek
        Vector2 direction = ((Vector2)target.transform.position - rb.position).normalized;

        rb.linearVelocity = direction * moveSpeed;
    }

    public void TakeDamage(int amount)
    {
        if (!Object.HasStateAuthority)
            return;

        Health -= amount;

        if (audioService != null && attackSound != null)
        {
            audioService.PlaySFX(attackSound);
        }

        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        rb.linearVelocity = Vector2.zero;

        // Acá puedes agregar animación, drops, etc.

        Runner.Despawn(Object);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        // agregamos el componente que haga daño o hacemos la colision con otro objeto
        if (collision.gameObject.CompareTag("bala"))
        {
            TakeDamage(5);
            return;
        }
        if(collision.gameObject.CompareTag("Player"))
        {
            // hacemos daño al jugador
            // usamos la propiedad AttackDamage
        }
    }
}
