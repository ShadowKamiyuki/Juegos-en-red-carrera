using Fusion;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using UnityEngine;

public class PlayerVoice : NetworkBehaviour
{
    private VoiceNetworkObject voiceNetworkObject;
    private Recorder recorder;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
            return;

        voiceNetworkObject = GetComponent<VoiceNetworkObject>();

        if (voiceNetworkObject == null)
        {
            Debug.LogError("No se encontró VoiceNetworkObject.");
            return;
        }

        recorder = voiceNetworkObject.RecorderInUse;

        if (recorder == null)
        {
            Debug.LogError("VoiceNetworkObject no tiene un Recorder asignado.");
            return;
        }

        recorder.InterestGroup = 0;
        recorder.TransmitEnabled = true;
        recorder.RecordingEnabled = true;

        Debug.Log("Recorder encontrado.");
        Debug.Log("Fuente de audio: " + recorder.SourceType);
        Debug.Log("Grabación habilitada: " + recorder.RecordingEnabled);
        Debug.Log("Transmisión habilitada: " + recorder.TransmitEnabled);
        Debug.Log("RecordingEnabled: " + recorder.RecordingEnabled);
        Debug.Log("TransmitEnabled: " + recorder.TransmitEnabled);
        Debug.Log("SourceType: " + recorder.SourceType);
        Debug.Log("MicrophoneType: " + recorder.MicrophoneType);
        Debug.Log("RecordWhenJoined: " + recorder.RecordWhenJoined);        
    }

    private void Update()
    {
        if (recorder == null)
            return;

        Debug.Log("Grabando: " + recorder.IsCurrentlyTransmitting);
    }
}