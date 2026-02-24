using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [field: Header("Movement Settings"), SerializeField]  public  float MoveSpeed {get; private set;}
    [field: SerializeField] public  float JumpForce{get; private set;}
    [field: SerializeField]  public  float RotationSpeed  {get; private set;}
    [field: Header("Ground Settings"), SerializeField]  public  LayerMask GroundMask {get; private set;}
    
    [field: SerializeField]  public  float GroundCheckDistance  {get; private set;}
    [field: SerializeField]  public  float PlayerHeight  {get; private set;}

    [field: Header("Restrictions Settings"), SerializeField]  public  float MaxSlopeAngle  {get; private set;}
    
    [field: SerializeField]  public  AnimationClip AttackClip {get; private set;}
    
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
    public AnimationClip AttackClip;

    public PlayerDataInstance(PlayerData data)
    {
        MoveSpeed = data.MoveSpeed;
        JumpForce = data.JumpForce;
        GroundMask = data.GroundMask;
        GroundCheckDistance = data.GroundCheckDistance;
        PlayerHeight = data.PlayerHeight;
        RotationSpeed = data.RotationSpeed;
        MaxSlopeAngle = data.MaxSlopeAngle;
        AttackClip = data.AttackClip;
    }

    public float GetLengthOfClip(AnimationClip clip)
    {
        return clip.length;
    }
    
}