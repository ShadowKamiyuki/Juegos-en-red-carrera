using Fusion;
using UnityEngine;

public class NetworkCameraController : NetworkBehaviour
{
    [SerializeField] private float speed = 2f;

    [Networked]
    public Vector3 CameraPosition { get; set; }

    public bool IsReady { get; private set; }

    public override void Spawned()
    {
        IsReady = true;

        Debug.Log("CAMARA NETWORK SPAWNED");

        if (HasStateAuthority)
        {
            CameraPosition = transform.position;

            Debug.Log("Posicion inicial: " + CameraPosition);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        CameraPosition += Vector3.up * speed * Runner.DeltaTime;

        speed += 0.0001f;
    }
}