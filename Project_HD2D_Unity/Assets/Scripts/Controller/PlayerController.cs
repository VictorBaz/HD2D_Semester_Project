using System;
using Character.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _PlayGround.Week01.Scripts
{
    public class PlayerController : MonoBehaviour
    {

        #region Variables

            private PlayerInputAction playerInputAction;
            private Vector2 moveInput;
            private Vector3 moveDIrection;
            private PlayerDataInstance playerData;
            
            
            [SerializeField] private Rigidbody rb;
            [SerializeField] private Transform cam;
            [SerializeField] private PlayerData playerDataRaw;
       

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            playerInputAction = new PlayerInputAction();

            playerData = playerDataRaw.Init();
        }

        private void OnEnable()
        {
            SetupActionsMap();
        }

        private void OnDisable()
        {
            StopActionsMap();
        }
        

        private void FixedUpdate()
        {
            HandleMovement();
        }

        #endregion

        #region Setup

        private void SetupActionsMap()
        {
            playerInputAction.Player.Move.performed +=  ReceiveMove;
            playerInputAction.Player.Move.canceled+=  ReceiveMove;
            playerInputAction.Player.Jump.performed += TryJump;
            playerInputAction.Enable();
        }
        

        private void StopActionsMap()
        {
            playerInputAction.Player.Move.performed -=  ReceiveMove;
            playerInputAction.Player.Move.canceled -=  ReceiveMove;
            playerInputAction.Player.Jump.performed -= TryJump;
            playerInputAction.Disable();
        }

        #endregion

        #region Inputs

        private void ReceiveMove(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }

        #endregion

        #region HandleMovement

        private void HandleMovement()
        {
            moveDIrection = cam.forward * moveInput.y;
            moveDIrection += cam.right * moveInput.x;
            moveDIrection.Normalize();
            
            moveDIrection *= playerData.MoveSpeed;
            
            rb.linearVelocity = new Vector3(
                moveDIrection.x,
                rb.linearVelocity.y,
                moveDIrection.z);
        }

        private void TryJump(InputAction.CallbackContext ctx)
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