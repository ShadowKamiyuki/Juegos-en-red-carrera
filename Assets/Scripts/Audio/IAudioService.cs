public interface IAudioService
{
    void PlayMusic(AudioDefinition definition);
    void PlaySFX(AudioDefinition definition);

    void StopMusic();
    void StopAll();
}
