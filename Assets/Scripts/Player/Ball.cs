using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float speed = 180f;

    private float angle;

    private void Update()
    {
        angle += speed * Time.deltaTime;

        float radians = angle * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(
            Mathf.Cos(radians),
            Mathf.Sin(radians)
        ) * distance;

        transform.position = (Vector2)player.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
}
