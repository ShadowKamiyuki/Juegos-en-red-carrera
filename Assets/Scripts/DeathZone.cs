using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("DeathZone Sound")]
    [SerializeField] private AudioDefinition DeathZoneSound;
    private IAudioService audioService;
    private void Start()
    {
        audioService = ServiceLocator.Get<IAudioService>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Player player = other.GetComponentInParent<Player>();

        if (audioService != null && DeathZoneSound != null)
        {
            audioService.PlaySFX(DeathZoneSound);
        }

        if (player == null)
            return;

        player.Kill();
    }
}