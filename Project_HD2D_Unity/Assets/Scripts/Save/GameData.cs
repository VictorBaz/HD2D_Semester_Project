using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public long LastUpdated;

    public PlayerSaveData PlayerData;

    public List<string> CompletedPuzzles = new();

    public string LastCompletedPuzzleId;
}