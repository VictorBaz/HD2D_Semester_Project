using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AnimationManager animationManager;
    [SerializeField] private LockOnSystem lockOnSystem;
    [SerializeField] private Transform cameraTransform;
    
    private Vector3 targetDirection;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        inputManager.OnJumpPressed += playerController.TryJump;
        inputManager.OnLockToggle += lockOnSystem.ToggleLock;
    }

    private void OnDisable()
    {
        inputManager.OnJumpPressed -= playerController.TryJump;
        inputManager.OnLockToggle -= lockOnSystem.ToggleLock;
    }

    private void Update()
    {
        lockOnSystem.UpdateLockRotation();
        CalculateTargetDirection();
    
        playerController.SetLockMode(lockOnSystem.IsLocked);
        playerController.UpdatePlayerController(cameraTransform, inputManager.MoveInput);

        animationManager.HandleAnimation(
            playerController.Rb.linearVelocity.magnitude, 
            CalculateAnimationInput()
        );
        
    }

    private void FixedUpdate()
    {
        playerController.UpdatePlayerControllerPhysics(targetDirection);
    }

    #endregion

    #region Private Methods

    private void CalculateTargetDirection()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        targetDirection = camForward * inputManager.MoveInput.y + 
                          camRight * inputManager.MoveInput.x;
        
        if (targetDirection.magnitude > 0.1f)
        {
            targetDirection.Normalize();
        }
    }
    
    private Vector2 CalculateAnimationInput()
    {
        //
        if (targetDirection.magnitude < 0.1f)
            return Vector2.zero;

        Vector3 reference = lockOnSystem.IsLocked
            ? transform.forward
            : cameraTransform.forward;

        Debug.DrawRay(transform.position,targetDirection, Color.red);
        
        reference.y = 0;
        reference.Normalize();
    
        float yaw = Vector3.SignedAngle(reference, targetDirection, Vector3.up);
        float yawRad = yaw * Mathf.Deg2Rad;

        
        return new Vector2(Mathf.Sin(yawRad), Mathf.Cos(yawRad));
    }

    

    #endregion
}