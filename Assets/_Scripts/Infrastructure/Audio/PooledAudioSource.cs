using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PooledAudioSource : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioSourcePool pool;

    private Coroutine playRoutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(AudioSourcePool ownerPool)
    {
        pool = ownerPool;
    }

    public void Play(AudioDefinition definition)
    {
        audioSource.ApplyDefinition(definition);

        audioSource.clip = definition.GetClip();

        if (audioSource.clip == null)
        {
            pool.Release(this);
            return;
        }

        audioSource.Play();

        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = StartCoroutine(ReturnWhenFinished());
    }

    private IEnumerator ReturnWhenFinished()
    {
        yield return new WaitUntil(() => !audioSource.isPlaying);

        pool.Release(this);
    }

    public void Stop()
    {
        audioSource.Stop();

        if (playRoutine != null)
            StopCoroutine(playRoutine);

        pool.Release(this);
    }

    public void ResetState()
    {
        audioSource.Stop();
        audioSource.clip = null;

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
    }
}