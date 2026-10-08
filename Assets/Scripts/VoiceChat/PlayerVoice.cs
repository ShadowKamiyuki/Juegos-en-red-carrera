using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using Fusion;


public class PlayerVoice : NetworkBehaviour
{
    public Recorder recorder;
    public FusionVoiceClient voiceClient;

   public override void Spawned()
    {
        if (!HasInputAuthority)
            return;

        recorder.InterestGroup = 1;
        voiceClient.Client.OpChangeGroups(null, new byte[] { 1 });
    }
}