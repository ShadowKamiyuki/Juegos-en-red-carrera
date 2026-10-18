using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PooledNetworkObjectProvider : Fusion.Behaviour, INetworkObjectProvider
{
    [Header("Pooling")]
    [SerializeField, Min(0)] private int maxPoolSizePerPrefab = 10;

    [SerializeField] private bool delayIfSceneManagerIsBusy = true;

    private readonly Dictionary<NetworkPrefabId, Queue<NetworkObject>> pools = new();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private NetworkObject GetPooledInstance(NetworkRunner runner, NetworkObject prefab, NetworkPrefabId prefabId)
    {
        if (!pools.TryGetValue(prefabId, out var pool))
        {
            pool = new Queue<NetworkObject>();
            pools.Add(prefabId, pool);
        }

        while (pool.Count > 0)
        {
            NetworkObject instance = pool.Dequeue();

            if (instance == null)
                continue;

            instance.gameObject.SetActive(true);
            return instance;
        }

        // No hay instancias disponibles: crear una nueva.
        return Instantiate(prefab);
    }

    private void ReturnToPool(NetworkPrefabId prefabId, NetworkObject instance)
    {
        if (instance == null)
            return;

        if (!pools.TryGetValue(prefabId, out var pool))
        {
            pool = new Queue<NetworkObject>();
            pools.Add(prefabId, pool);
        }

        // Si el pool está lleno, destruir el objeto excedente.
        if (maxPoolSizePerPrefab > 0 && pool.Count >= maxPoolSizePerPrefab)
        {
            Destroy(instance.gameObject);
            return;
        }

        ResetLocalState(instance);

        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }

    public NetworkObjectAcquireResult AcquireInstance(NetworkRunner runner, in NetworkObjectAcquireContext context, out NetworkObject instance)
    {
        instance = null;

        if (delayIfSceneManagerIsBusy && runner.SceneManager != null && runner.SceneManager.IsBusy)
        {
            return NetworkObjectAcquireResult.Retry;
        }

        // Los objetos de escena los gestiona el Scene Manager.
        if (context.TypeId.IsSceneObject)
        {
            if (context.AttachableInstance != null)
            {
                instance = context.AttachableInstance;
                return NetworkObjectAcquireResult.Success;
            }

            return NetworkObjectAcquireResult.Retry;
        }

        if (!context.TypeId.IsPrefab)
            return NetworkObjectAcquireResult.Failed;

        NetworkPrefabId prefabId = context.TypeId.AsPrefabId;

        NetworkObject prefab;

        try
        {
            prefab = runner.Prefabs.Load(prefabId, isSynchronous: context.IsSynchronous);
        }
        catch (Exception ex)
        {
            Debug.LogError($"No se pudo cargar el prefab de red: {ex}");

            return NetworkObjectAcquireResult.Failed;
        }

        if (prefab == null)
            return NetworkObjectAcquireResult.Retry;

        instance = GetPooledInstance(runner, prefab, prefabId);

        if (instance == null)
            return NetworkObjectAcquireResult.Failed;

        if (context.DontDestroyOnLoad)
            runner.MakeDontDestroyOnLoad(instance.gameObject);
        else
            runner.MoveToRunnerScene(instance.gameObject);

        runner.Prefabs.AddInstance(prefabId);

        return NetworkObjectAcquireResult.Success;
    }

    public void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
    {
        NetworkObject instance = context.Object;

        if (!context.IsBeingDestroyed)
        {
            if (context.TypeId.Kind == NetworkTypeIdKind.Prefab)
            {
                ReturnToPool(context.TypeId.AsPrefabId, instance);
            }
            else if (context.TypeId.Kind == NetworkTypeIdKind.PrefabNested)
            {
                if (instance != null)
                    instance.gameObject.SetActive(false);
            }
            else if (instance != null)
            {
                Destroy(instance.gameObject);
            }
        }

        if (context.TypeId.IsPrefab)
        {
            runner.Prefabs.RemoveInstance(context.TypeId.AsPrefabId);
        }
    }

    public void Shutdown(NetworkRunner runner)
    {
        // Liberar las instancias que quedaron guardadas.
        foreach (var pool in pools.Values)
        {
            while (pool.Count > 0)
            {
                NetworkObject instance = pool.Dequeue();

                if (instance != null)
                    Destroy(instance.gameObject);
            }
        }

        pools.Clear();

        var prefabs = runner.Prefabs;

        if (prefabs?.Options.UnloadUnusedPrefabsOnShutdown == true)
            prefabs.UnloadUnreferenced(includeIncompleteLoads: true);
    }

    public NetworkPrefabId GetPrefabId(NetworkRunner runner, NetworkObjectGuid prefabGuid)
    {
        return runner.Prefabs.GetId(prefabGuid);
    }

    private static void ResetLocalState(NetworkObject instance)
    {
        // Física 2D.
        foreach (var body in instance.GetComponentsInChildren<Rigidbody2D>(true))
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }
    }
}