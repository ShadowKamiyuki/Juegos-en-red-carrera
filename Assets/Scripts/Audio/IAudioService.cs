public interface IAudioService
{
    void PlayMusic(AudioDefinition definition);
    void PlaySFX(AudioDefinition definition);
    void PlayVoice(AudioDefinition definition);

    void StopMusic();
    void StopAll();
}
