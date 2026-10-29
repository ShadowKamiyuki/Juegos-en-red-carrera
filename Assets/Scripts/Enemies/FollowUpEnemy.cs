using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class FollowUpEnemy : NetworkBehaviour
{
    [Header("Target")]
    [SerializeField] private float detectionRadious = 5;
    [SerializeField] private LayerMask playerLayer;

    [Header("Lag Compensation")]
    [SerializeField] private float collisionCheckRadius = 0.5f;
    [SerializeField] private LayerMask playerHitboxLayer;

    [Header("Enemy settings")]
    [SerializeField] private byte initialHealth = 10;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private AudioDefinition attackSound;

    [Networked] public byte Health { get; set; }

    private Rigidbody2D rb;
    private NetworkObject target;
    private IAudioService audioService;
    private readonly List<LagCompensatedHit> lagCompensationHits = new List<LagCompensatedHit>();

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
        if (Object.HasStateAuthority)
        {
            Health = initialHealth;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        GetClosestPlayer();
        FollowPlayer();

        CheckPlayerWithLagCompensation();
    }

    private void CheckPlayerWithLagCompensation()
    {
        if (target == null)
            return;

        Player player = target.GetComponent<Player>();

        if (player == null)
            return;

        int hitsFound = Runner.LagCompensation.OverlapSphere(
            transform.position,
            collisionCheckRadius,
            target.InputAuthority,
            lagCompensationHits,
            playerHitboxLayer,
            HitOptions.SubtickAccuracy
        );

        for (int i = 0; i < hitsFound; i++)
        {
            if (lagCompensationHits[i].Hitbox == null)
                continue;

            Player hitPlayer =
                lagCompensationHits[i].Hitbox.GetComponentInParent<Player>();

            if (hitPlayer == player)
            {
                hitPlayer.Kill();
                return;
            }
        }
    }

    private void GetClosestPlayer()
    {
        //necesitamos una estrategia para buscar al jugador correcto
        Collider2D[] players = Physics2D.OverlapCircleAll(rb.position, detectionRadious, playerLayer);

        target = null;

        float closestDistance = Mathf.Infinity;

        foreach (Collider2D player in players)
        {
            NetworkObject networkObject = player.GetComponentInParent<NetworkObject>();

            if (networkObject == null)
                continue;

            if (networkObject == Object)
                continue;

            if (!networkObject.CompareTag("Player"))
                continue;

            float distance = Vector2.Distance(rb.position, networkObject.transform.position);

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

    public void TakeDamage(byte amount)
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

        Runner.Despawn(Object);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            //hacemos daño al jugador
            //usamos la propiedad AttackDamage
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null)
            {
                player.Kill();
            }
        }
    }
}
