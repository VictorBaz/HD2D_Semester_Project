using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [field: SerializeField]  public  float MoveSpeed {get; private set;}
    [field: SerializeField] public  float JumpForce{get; private set;}
    [field: SerializeField]  public  LayerMask GroundMask {get; private set;}
    
    [field: SerializeField]  public  float GroundCheckDistance  {get; private set;}
    [field: SerializeField]  public  float PlayerHeight  {get; private set;}
    [field: SerializeField]  public  float RotationSpeed  {get; private set;}
    
    [field: SerializeField]  public  float MaxSlopeAngle  {get; private set;}

    public PlayerDataInstance Init()
    {
        return new PlayerDataInstance(this);
    }
}

    
[System.Serializable]
public class PlayerDataInstance
{
    public float MoveSpeed;
    public float JumpForce;
    public LayerMask GroundMask;
    public float GroundCheckDistance;
    public float PlayerHeight;
    public float RotationSpeed;
    public float MaxSlopeAngle;

    public PlayerDataInstance(PlayerData data)
    {
        MoveSpeed = data.MoveSpeed;
        JumpForce = data.JumpForce;
        GroundMask = data.GroundMask;
        GroundCheckDistance = data.GroundCheckDistance;
        PlayerHeight = data.PlayerHeight;
        RotationSpeed = data.RotationSpeed;
        MaxSlopeAngle = data.MaxSlopeAngle;
    }
}