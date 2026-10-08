using Photon.Voice.Unity;
using UnityEngine;

public class Chat : MonoBehaviour
{
    Recorder recorder;
    public void SetMute(bool mute)
    {
        recorder.TransmitEnabled = !mute;
    }

    void Update()
    {
        recorder.TransmitEnabled = Input.GetKey(KeyCode.V);
    }
}