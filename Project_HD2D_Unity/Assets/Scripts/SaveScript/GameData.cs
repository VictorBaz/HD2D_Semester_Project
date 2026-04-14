using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long LastUpdated;
    
    public int DeathCount;
    public int EnergyPoint;

    public List<bool> ParasitesAlive;
    
    public GameData()
    {
        this.DeathCount = 0;
        this.EnergyPoint = 10;
        
        this.ParasitesAlive = new List<bool> { true, true };
    }

    public int GetPercentageComplete()
    {
        if (ParasitesAlive == null || ParasitesAlive.Count == 0)
        {
            return 0;
        }

        int deadCount = 0;
        foreach (bool isAlive in ParasitesAlive)
        {
            if (!isAlive)
            {
                deadCount++;
            }
        }

        return (int)((float)deadCount / ParasitesAlive.Count * 100);
    }
}