using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class AudioSourcePool : MonoBehaviour
{
    [SerializeField] private PooledAudioSource prefab;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 30;

    private ObjectPool<PooledAudioSource> pool;

    private readonly HashSet<PooledAudioSource> activeSources = new();

    private void Awake()
    {
        pool = new ObjectPool<PooledAudioSource>(
            Create,
            OnGet,
            OnRelease,
            OnDestroyPoolObject,
            true,
            defaultCapacity,
            maxSize);
    }

    private PooledAudioSource Create()
    {
        PooledAudioSource source = Instantiate(prefab, transform);
        source.Initialize(this);
        source.gameObject.SetActive(false);
        return source;
    }

    private void OnGet(PooledAudioSource source)
    {
        activeSources.Add(source);
        source.gameObject.SetActive(true);
    }

    private void OnRelease(PooledAudioSource source)
    {
        activeSources.Remove(source);

        source.ResetState();
        source.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(PooledAudioSource source)
    {
        Destroy(source.gameObject);
    }

    public PooledAudioSource Get() => pool.Get();

    public void Release(PooledAudioSource source) => pool.Release(source);

    public void StopAll()
    {
        foreach (var source in activeSources.ToArray())
        {
            source.Stop();
        }
    }
}
