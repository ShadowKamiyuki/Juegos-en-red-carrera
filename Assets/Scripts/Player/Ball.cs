using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float speed = 180f;
    [SerializeField] private Player owner;

    private float angle;

    private void Update()
    {
        angle += speed * Time.deltaTime;

        float radians = angle * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * distance;

        transform.position = (Vector2)player.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            DownEnemy enemy = collision.GetComponent<DownEnemy>();

            if (enemy == null || enemy.Object == null)
                return;

            if (!enemy.Object.HasStateAuthority)
                return;

            enemy.Runner.Despawn(enemy.Object);
            return;
        }

        if (collision.CompareTag("Player") && owner != null && owner.CanPvp)
        {
            Player target = collision.GetComponent<Player>();

            if (target != null)
                target.Kill();
        }
    }
}
