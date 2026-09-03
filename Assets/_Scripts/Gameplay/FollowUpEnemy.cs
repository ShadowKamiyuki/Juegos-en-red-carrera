using Fusion;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class FollowUpEnemy : NetworkBehaviour
{
    [Header("Target")]
    [SerializeField] private GameObject target;

    [Header("Enemy settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float health;
    [SerializeField] private float attackDamage;

    private Rigidbody2D rb;

    // properties for Fusion
    [Networked] public float Health { get { return health; } set { health = value; } }
    [Networked] public float AttackDamage => attackDamage;
    [Networked] public float MoveSpeed => moveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        // using Steearing behaviour seek
        Vector3 direction = target.transform.position - transform.position;
        direction.z = 0f; // only 2D

        rb.linearVelocity = direction * moveSpeed;
    }

    public void TakeDamage(int amount)
    {
        if (!Object.HasStateAuthority)
            return;

        Health -= amount;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        // agregamos el componente que haga daño o hacemos la colision con otro objeto
    }
}
