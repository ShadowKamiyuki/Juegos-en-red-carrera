using UnityEngine;

public class AudioManager : MonoBehaviour, IAudioService
{
    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSourcePool sfxPool;

    private AudioDefinition currentMusic;

    public void PlayMusic(AudioDefinition definition)
    {
        if (definition == null)
            return;

        if (definition == currentMusic && musicSource.isPlaying)
            return;

        currentMusic = definition;
        AudioClip clip = definition.GetClip();

        if (clip == null)
            return;

        musicSource.ApplyDefinition(definition);
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioDefinition definition)
    {
        if (definition == null)
            return;

        PooledAudioSource source = sfxPool.Get();

        source.Play(definition);
    }

    public void StopMusic()
    {
        musicSource.Stop();
        currentMusic = null;
    }

    public void StopAll()
    {
        musicSource.Stop();

        sfxPool.StopAll();

        currentMusic = null;
    }
}
