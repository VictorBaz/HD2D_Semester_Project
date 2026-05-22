using System;
using UnityEngine;

public class UnlockParryZone : MonoBehaviour
{
    private bool unlockParry = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (unlockParry || !other.CompareTag(GameConstants.PLAYER_TAG)) return;
        
        unlockParry = true;
            
        other.GetComponent<PlayerActionHandler>().UnlockParry();
    }
}
