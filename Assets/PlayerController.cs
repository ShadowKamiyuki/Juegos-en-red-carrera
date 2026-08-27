using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;

    public override void FixedUpdateNetwork()
    {
        Debug.Log(
            "PLAYER | StateAuthority: " + Object.HasStateAuthority +
            " | InputAuthority: " + Object.HasInputAuthority
        );

        if (!Object.HasStateAuthority)
            return;

        if (GetInput(out NetworkInputData data))
        {
            Debug.Log("INPUT RECIBIDO POR SERVER: " + data.direction);

            Vector3 movement = new Vector3(
                data.direction.x,
                0,
                data.direction.y
            );

            transform.position += movement * speed * Runner.DeltaTime;
        }
        else
        {
            Debug.Log("NO SE RECIBIO INPUT");
        }
    }
}