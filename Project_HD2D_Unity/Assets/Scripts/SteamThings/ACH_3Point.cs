using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class ACH_Trigger : MonoBehaviour
{
    
    private SteamAchivementManager ACHManager;
    [SerializeField] private string achievementId;
    [SerializeField] private int sheepCounter = 0;

    private void Start()
    {
        ACHManager = Object.FindFirstObjectByType<SteamAchivementManager>();
    }

    void Update()
    {
        ACH_Unlock();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemySheepManager>(out EnemySheepManager sheep))
        {
            sheepCounter++;
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
        
        if (sheepCounter == 3 && !ACHManager.IsThisAchievementUnlocked(achievementId))
        {
            ACHManager.UnlockAchivement(achievementId);
        }
    }
}
