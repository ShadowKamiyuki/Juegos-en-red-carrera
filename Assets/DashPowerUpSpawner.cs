using Fusion;
using UnityEngine;

public class DashPowerUpSpawner : NetworkBehaviour
{
    [Header("Power Up")]
    [SerializeField] private NetworkPrefabRef dashPowerUpPrefab;

    [Header("Spawn")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float spawnHeight = 2f;

    [Header("Cantidad")]
    [SerializeField] private int maxSpawns = 3;

    [Header("Tiempo")]
    [SerializeField] private float firstSpawnTime = 3f;
    [SerializeField] private float spawnInterval = 20f;

    private int spawnedCount;
    private float spawnTimer;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
            return;

        spawnedCount = 0;
        spawnTimer = firstSpawnTime;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (spawnedCount >= maxSpawns)
            return;

        spawnTimer -= Runner.DeltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnPowerUp();

            spawnedCount++;

            spawnTimer = spawnInterval;
        }
    }

    private void SpawnPowerUp()
    {
        if (mainCamera == null)
        {
            Debug.LogError("No se encontró la cámara.");
            return;
        }

        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float randomX = Random.Range(
            mainCamera.transform.position.x - cameraWidth,
            mainCamera.transform.position.x + cameraWidth
        );

        float spawnY =
            mainCamera.transform.position.y +
            cameraHeight +
            spawnHeight;

        Vector3 spawnPosition = new Vector3(
            randomX,
            spawnY,
            0f
        );

        Runner.Spawn(
            dashPowerUpPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Debug.Log("Dash Power Up spawneado.");
    }
}