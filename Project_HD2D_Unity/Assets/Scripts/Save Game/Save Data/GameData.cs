using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public long LastUpdated;

    public PlayerSaveData PlayerData = new PlayerSaveData();

    public List<string> CompletedPuzzles = new();
    
    public List<RootSaveData> rootDataList = new();
    public List<SapSaveData> sapDataList = new();
    public List<ParasiteSaveData> parasiteDataList = new();

    public string LastCompletedPuzzleId;
    public string LastVisitedPuzzleId;
}