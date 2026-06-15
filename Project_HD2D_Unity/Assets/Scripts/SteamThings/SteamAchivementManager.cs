using UnityEngine;
using System;
using System.Linq;
using Steamworks;

public class SteamAchivementManager : MonoBehaviour
{
    
    private const string PLATINUM_ID = "ACH_100_PRCT";
    private const int SteamGameID = 4782970;
    
    #region Singleton
    
        public static SteamAchivementManager Instance;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
        
    #endregion

    private void Start()
    {
        SteamStart();
    }

    private void SteamStart()
    {

        if (SteamGameID != 4782970)
        {
            SteamCheckToDestroy();
        }
        
        try
        {
            SteamClient.Init(SteamGameID);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            SteamCheckToDestroy();
        }
    }

    private void SteamCheckToDestroy()
    { 
        #if UNITY_EDITOR
            Destroy(gameObject);
        #else
            Application.Quit();
        #endif
    }

    void Update()
    {
        SteamClient.RunCallbacks();
    }

    public void UnlockAchivement(string achievementID)
    {
        var ach = new Steamworks.Data.Achievement(achievementID);
        ach.Trigger();
        SteamUserStats.StoreStats();
        Debug.Log($"Achivement {achievementID} has been unlocked");
        CheckForPlatinum();
    }

    public void CheckForPlatinum()
    {
        bool allUnlocked = SteamUserStats.Achievements
            .Where(ach => ach.Identifier != PLATINUM_ID)
            .All(ach => ach.State);

        if (allUnlocked)
        {
            UnlockPlatinumAchievement();
        }
    }
    
    private void UnlockPlatinumAchievement()
    {
        var ach = new Steamworks.Data.Achievement(PLATINUM_ID);
        ach.Trigger(); // Sauvegarde automatiquement sur Steam (apply = true par défaut)
        Debug.Log("Félicitations, Platine débloqué !");
    }
    
    public bool IsThisAchievementUnlocked(string achievementID)
    {
        var ach = new Steamworks.Data.Achievement(achievementID);
        Debug.Log($"Achivement {achievementID} state is {ach.State}");
        
        return ach.State;
    }
    
    public void UnlockAllExceptPlatinum()
    {
        foreach (var ach in SteamUserStats.Achievements)
        {
            if (ach.Identifier != PLATINUM_ID && !ach.State)
            {
                ach.Trigger();
                Debug.Log($"Succès débloqué de force : {ach.Name}");
            }
        }
        
        CheckForPlatinum();
    }
    
    public void ClearAchivement(string achievementID)
    {
        var ach = new Steamworks.Data.Achievement(achievementID);
        ach.Clear();
        SteamUserStats.StoreStats();
        Debug.Log($"Achivement {achievementID} has been cleared");
    }
    
    public void ClearEveryAchivements()
    {
        Debug.Log("CLEARING ACHIVEMENT");
        foreach (var ach in SteamUserStats.Achievements)
        {
            if (ach.State)
            {
                ach.Clear();
            }
        }

        SteamUserStats.StoreStats();
    }

    void OnApplicationQuit()
    {
        #if UNITY_EDITOR
        ClearEveryAchivements();
        #endif
        
        SteamClient.Shutdown();
    }
}