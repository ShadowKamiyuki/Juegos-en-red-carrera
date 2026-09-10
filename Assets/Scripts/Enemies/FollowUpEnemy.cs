using Fusion;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class FollowUpEnemy : NetworkBehaviour
{
    [Header("Target")]
    [SerializeField] private float detectionRadious = 5;
    [SerializeField] private LayerMask playerLayer;

    [Header("Enemy settings")]
    [Networked] public float Health { get; set; }
    [Networked] public float AttackDamage { get; set; }
    [Networked] public float MoveSpeed { get; set; }

    [SerializeField] private AudioDefinition attackSound;

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

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        GetClosestPlayer();
        FollowPlayer();
    }

    private Vector2 GetClosestPlayer()
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

        return default;
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

        rb.linearVelocity = direction * MoveSpeed;
    }

    public void TakeDamage(int amount)
    {
        if (!Object.HasStateAuthority)
            return;

        Health -= amount;

        audioService.PlaySFX(attackSound);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        // agregamos el componente que haga daño o hacemos la colision con otro objeto
        if (collision.gameObject.CompareTag("bala"))
        {
            TakeDamage(5);
        }
        if(collision.gameObject.CompareTag("Player"))
        {
            // hacemos daño al jugador
            // usamos la propiedad AttackDamage
        }
    }
}
