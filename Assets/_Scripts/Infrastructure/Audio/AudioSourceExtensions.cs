using UnityEngine;

public static class AudioSourceExtensions
{
    public static void ApplyDefinition(this AudioSource source, AudioDefinition definition)
    {
        source.outputAudioMixerGroup = definition.MixerGroup;
        source.loop = definition.Loop;
        source.priority = definition.Priority;
        source.volume = definition.Volume;
        source.pitch = definition.GetPitch();
        source.spatialBlend = definition.SpatialBlend;
        source.rolloffMode = definition.RolloffMode;
        source.minDistance = definition.MinDistance;
        source.maxDistance = definition.MaxDistance;
        source.spread = definition.Spread;
        source.dopplerLevel = definition.DopplerLevel;
        source.reverbZoneMix = definition.ReverbZoneMix;
    }
}
