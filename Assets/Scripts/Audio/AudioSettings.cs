using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void UI_SetMasterVolume(float value)
    {
        SetProperty("MasterVolume", value);
    }

    public void UI_SetMusicVolume(float value)
    {
        SetProperty("MusicVolume", value);
    }

    public void UI_SetSFXVolume(float value)
    {
        SetProperty("SoundEffectsVolume", value);
    }

    public void UI_SetVoicesVolume(float value)
    {
        SetProperty("VoicesVolume", value);
    }

    private void SetProperty(string name, float value)
    {
        value = Mathf.Max(value, 0.0001f);
        audioMixer.SetFloat(name, Mathf.Log10(value) * 20);
    }
}
