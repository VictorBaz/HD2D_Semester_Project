[System.Serializable]
public class PlayerSaveData
{
    public int Life;
    public int Energy;
    public int Sap;

    public PlayerSaveData(PlayerDataInstance data)
    {
        Life = data.Life;
        Energy = data.Energy;
        Sap = data.Sap;
    }
}