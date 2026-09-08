using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Audio Definition", menuName = "Project/Audio/Audio Definition")]
public class AudioDefinition : ScriptableObject
{
    [Header("General")]
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioMixerGroup mixerGroup;

    [Header("Playback")]
    [SerializeField] private bool loop;

    [Header("Volume")]
    [Range(0f, 1f), SerializeField] private float volume = 1f;
    [Range(-3f, 3f), SerializeField] private float pitch = 1f;
    [Range(-0.5f, 0.5f), SerializeField] private float randomPitch;

    [Header("Spatial")]
    [Range(-1f, 1f), SerializeField] private float panStereo;
    [Range(0f, 1f), SerializeField] private float spatialBlend;
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    [SerializeField] private float minDistance = 1;
    [SerializeField] private float maxDistance = 500;
    [SerializeField] private float spread;
    [SerializeField] private float dopplerLevel = 1;
    [SerializeField] private float reverbZoneMix = 1;

    [Header("Priority")]
    [Range(0, 256)]
    [SerializeField] private int priority = 128;

    public bool Loop => loop;
    public AudioMixerGroup MixerGroup => mixerGroup;
    public int Priority => priority;
    public float Volume => volume;
    public float GetPitch()
    {
        return pitch + Random.Range(-randomPitch, randomPitch);
    }
    public float PanStereo => panStereo;
    public float SpatialBlend => spatialBlend;
    public AudioRolloffMode RolloffMode => rolloffMode;
    public float MinDistance => minDistance;
    public float MaxDistance => maxDistance;
    public float Spread => spread;
    public float DopplerLevel => dopplerLevel;
    public float ReverbZoneMix => reverbZoneMix;


    public AudioClip GetClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        if (clips.Length == 1)
            return clips[0];

        return clips[Random.Range(0, clips.Length)];
    }
}