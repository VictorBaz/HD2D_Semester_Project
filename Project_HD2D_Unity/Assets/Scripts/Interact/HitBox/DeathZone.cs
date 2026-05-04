using UnityEngine;

public class DeathZone : MonoBehaviour
{
    //not really death zone but player is tp to secure place
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(GameConstants.PLAYER_TAG)) return;

        var player = other.GetComponentInParent<PlayerManager>();

        if (player == null) return;
        
        player.TriggerRespawn(false);
    }
}