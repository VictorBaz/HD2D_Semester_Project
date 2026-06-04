using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class ACH_Trigger : MonoBehaviour
{
    
    private SteamAchivementManager ACHManager;
    private string achievementId = "ACH_SHEEP_LAUNCH";
    private bool noMoreSteamCall = false;
    [SerializeField] private int sheepCounter = 0;

    private void Start()
    {
        ACHManager = SteamAchivementManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemySheepManager>(out EnemySheepManager sheep))
        {
            sheepCounter++;
            
            if (sheepCounter == 3)
                ACH_Unlock();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<EnemySheepManager>(out EnemySheepManager sheep))
        {
            sheepCounter--;
        }
    }

    void ACH_Unlock()
    {
        if (ACHManager == null)
            return;
        if (ACHManager.IsThisAchievementUnlocked(achievementId))
        {
            noMoreSteamCall = true;
            return;
        }
        
        if (!ACHManager.IsThisAchievementUnlocked(achievementId))
        {
            ACHManager.UnlockAchivement(achievementId);
        }
    }
}
