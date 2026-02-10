using System;
using Character.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class CharacterMovements : MonoBehaviour
    {
        #region Link

            public CharacterManager CharManager;

        #endregion
        
        #region Variables

            [HideInInspector]
            public Vector2 moveInput;
            private Vector3 moveDirection;
            private PlayerDataInstance playerData;
            
            
            [SerializeField] private Rigidbody rb;
            [SerializeField] private Transform cam;
            [SerializeField] private PlayerData playerDataRaw;
       

        #endregion

        #region Unity Lifecycle

            private void Awake()
            {
                playerData = playerDataRaw.Init();
            }

            private void OnEnable()
            {
                CharManager.CharInput.SetupActionsMap();
            }

            private void OnDisable()
            {
                CharManager.CharInput.StopActionsMap();
            }
            

            private void FixedUpdate()
            {
                HandleMovement();
            }

        #endregion

        #region Inputs

            public void ReceiveMove(InputAction.CallbackContext ctx)
            {
                moveInput = ctx.ReadValue<Vector2>();
            }

        #endregion

        #region HandleMovement

            private void HandleMovement()
            {
                moveDirection = cam.forward * moveInput.y;
                moveDirection += cam.right * moveInput.x;
                moveDirection.Normalize();
                
                moveDirection *= playerData.MoveSpeed;
                
                rb.linearVelocity = new Vector3(
                    moveDirection.x,
                    rb.linearVelocity.y,
                    moveDirection.z);
            }

            public void TryJump(InputAction.CallbackContext ctx)
            {

                if (Physics.Raycast(
                        transform.position, 
                        -Vector3.up,
                        playerData.GroundRadius,
                        playerData.GroundMask))
                {
                    Jump();
                }
                
            }

            private void Jump()
            {
                rb.AddForce(new Vector3(
                    0,
                    playerData.JumpForce,
                    0),
                    ForceMode.Impulse);
            }

        #endregion
        
        
    }
}


