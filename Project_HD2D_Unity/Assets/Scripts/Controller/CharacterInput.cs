using UnityEngine;

namespace Character
{
    public class CharacterInput : MonoBehaviour
    {

        #region Link

            public CharacterManager CharManager;

        #endregion
    
        #region Variables

        private PlayerInputAction playerInputAction;
        

        #endregion
    
        #region Unity Lifecycle

            private void Awake()
            {
                playerInputAction = new PlayerInputAction();
                
            }

            private void OnEnable()
            {
                
            }

            private void OnDisable()
            {
                
            }
                    

            private void FixedUpdate()
            {
                
            }

        #endregion
    
        #region Setup

            public void SetupActionsMap()
            {
                playerInputAction.Player.Move.performed +=  CharManager.CharMovements.ReceiveMove;
                playerInputAction.Player.Move.canceled+=  CharManager.CharMovements.ReceiveMove;
                playerInputAction.Player.Jump.performed += CharManager.CharMovements.TryJump;
                playerInputAction.Enable();
            }
                

            public void StopActionsMap()
            {
                playerInputAction.Player.Move.performed -=  CharManager.CharMovements.ReceiveMove;
                playerInputAction.Player.Move.canceled -=  CharManager.CharMovements.ReceiveMove;
                playerInputAction.Player.Jump.performed -= CharManager.CharMovements.TryJump;
                playerInputAction.Disable();
            }

        #endregion
    }
}

