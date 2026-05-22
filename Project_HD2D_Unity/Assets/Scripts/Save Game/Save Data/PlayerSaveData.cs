using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public int Life;
    public int Energy;
    public int Sap;
    public Vector3 Checkpoint;

    public PlayerSaveData(PlayerDataInstance data, Vector3 checkpoint)
    {
        Life = data.Life;
        Energy = data.Energy;
        Sap = data.Sap;
        Checkpoint = checkpoint;
    }

    public PlayerSaveData()
    {
        
    }
}