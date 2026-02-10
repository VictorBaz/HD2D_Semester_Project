using UnityEngine;

namespace Character.Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData", order = 1)]
    public class PlayerData : ScriptableObject
    {
        [field: SerializeField]  public  float MoveSpeed {get; private set;}
        [field: SerializeField] public  float JumpForce{get; private set;}
        [field: SerializeField]  public  LayerMask GroundMask {get; private set;}
        [field: SerializeField]  public  float GroundRadius {get; private set;}

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
        public float GroundRadius;

        public PlayerDataInstance(PlayerData data)
        {
            MoveSpeed = data.MoveSpeed;
            JumpForce = data.JumpForce;
            GroundMask = data.GroundMask;
            GroundRadius = data.GroundRadius;
        }
    }
}