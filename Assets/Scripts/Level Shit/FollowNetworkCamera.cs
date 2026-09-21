using UnityEngine;

public class FollowNetworkCamera : MonoBehaviour
{
    private NetworkCameraController controller;

    private void LateUpdate()
    {
        if (controller == null)
        {
            controller = FindFirstObjectByType<NetworkCameraController>();
            return;
        }

        if (!controller.IsReady)
            return;

        Vector3 position = transform.position;

        position.y = controller.CameraPosition.y;

        transform.position = position;
    }
}