using Photon.Voice.Unity;
using Photon.Voice.Fusion;
using Fusion;
using UnityEngine;

public class PlayerVoice : NetworkBehaviour
{
    [SerializeField] private Recorder recorder;

    public override void Spawned()
    {
        // Solo el jugador local usa su micrófono.
        if (!Object.HasInputAuthority)
            return;

        if (recorder != null)
        {
            recorder.InterestGroup = 0;
            recorder.RecordingEnabled = true;
            recorder.TransmitEnabled = true;
        }
    }
}