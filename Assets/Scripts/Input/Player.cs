using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector2 movement = new Vector2(data.Direction.x, data.Direction.y);

            transform.Translate(movement * speed * Runner.DeltaTime);
        }
    }

    public void Kill()
    {
        Runner.Despawn(Object);
    }
}
