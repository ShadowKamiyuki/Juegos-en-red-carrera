using Fusion;
using UnityEngine;

public class DownEnemySpawner : NetworkBehaviour
{
    [Header("Enemy")]
    [SerializeField] private NetworkPrefabRef enemyPrefab;
    [SerializeField] private NetworkPrefabRef enemyPrefab2;
    [SerializeField] private NetworkPrefabRef enemyPrefab3;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float margin = 1f;

    private TickTimer spawnTimer;
    private Camera mainCamera;

    public override void Spawned()
    {
        mainCamera = Camera.main;

        if (Object.HasStateAuthority)
        {
            spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnInterval);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (!spawnTimer.Expired(Runner))
            return;

        SpawnEnemy();

        spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnInterval);
    }

    private void SpawnEnemy()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
                return;
        }

        // Bordes de la cámara
        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f));

        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        // X aleatoria dentro del ancho visible
        float randomX = Random.Range(leftEdge.x, rightEdge.x);

        Vector3 spawnPosition = new Vector3(randomX, leftEdge.y + margin, 0f);

        NetworkPrefabRef selectedEnemy;

        int randomEnemy = Random.Range(0, 3);

        if (randomEnemy == 0)
        {
            selectedEnemy = enemyPrefab;
        }
        else if (randomEnemy == 1)
        {
            selectedEnemy = enemyPrefab2;
        }
        else
        {
            selectedEnemy = enemyPrefab3;
        }

        Runner.Spawn(selectedEnemy, spawnPosition, Quaternion.identity);
    }
}