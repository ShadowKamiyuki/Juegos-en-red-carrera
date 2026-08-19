using UnityEngine;

public class AudioManager : MonoBehaviour, IAudioService
{
    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSourcePool sfxPool;
    [SerializeField] private AudioSource voiceSource;

    private AudioDefinition currentMusic;
    private AudioDefinition currentVoice;

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

    public void PlayVoice(AudioDefinition definition)
    {
        if (definition == null)
            return;

        if (definition == currentVoice && voiceSource.isPlaying)
            return;

        currentVoice = definition;
        AudioClip clip = definition.GetClip();

        if (clip == null)
            return;

        voiceSource.ApplyDefinition(definition);
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        currentMusic = null;
        currentVoice = null;
    }

    public void StopAll()
    {
        musicSource.Stop();
        voiceSource.Stop();

        sfxPool.StopAll();

        currentMusic = null;
        currentVoice = null;
    }
}
