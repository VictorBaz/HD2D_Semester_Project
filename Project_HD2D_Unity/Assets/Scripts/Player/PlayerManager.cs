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
    [SerializeField] private Transform visualTransform; 
    [SerializeField] private PlayerCursor playerCursor; 
    
    private Vector3 targetDirection;
    private Vector2 blendInput;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        inputManager.OnLockToggle += lockOnSystem.ToggleLock;

        inputManager.OnAttackMelee += playerController.TryAttackMelee;
        
        playerController.OnAttackMelee  += animationManager.AttackMelee;
        
        inputManager.OnJumpPressed += playerController.TryJump;
        playerController.OnJump += HandleJump;
        
        inputManager.OnShoot += playerController.TryShoot;
        playerController.OnSimpleShoot += HandleSimpleShoot;
        playerController.OnChargeShoot += HandleChargeShoot;
    }

    private void OnDisable()
    {
        inputManager.OnLockToggle -= lockOnSystem.ToggleLock;
        
        inputManager.OnAttackMelee -= playerController.TryAttackMelee;
        
        playerController.OnAttackMelee  -= animationManager.AttackMelee;
        
        inputManager.OnJumpPressed -= playerController.TryJump;
        playerController.OnJump -= HandleJump;
        
        inputManager.OnShoot -= playerController.TryShoot;
        playerController.OnSimpleShoot -= HandleSimpleShoot;
        playerController.OnChargeShoot -= HandleChargeShoot;
    }

    private void Update()
    {
        lockOnSystem.UpdateLockRotation();
        CalculateTargetDirection();
    
        playerController.SetLockMode(lockOnSystem.IsLocked);
        playerController.UpdatePlayerController(cameraTransform, inputManager.MoveInput);

        blendInput = GetBlendTreeInput(playerController.IsAttacking);
        
        animationManager.HandleAnimation(
            playerController.Rb.linearVelocity.magnitude, 
            blendInput,
            playerController.IsGrounded
        );
        
        playerCursor.HandleRotation(inputManager.ShootInput);
        
    }

    private void FixedUpdate()
    {
        playerController.UpdatePlayerControllerPhysics(targetDirection);
    }

    private void LateUpdate()
    {
        playerCursor.FollowPlayer();
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
    
    

    Vector2 GetBlendTreeInput(bool isAttacking)
    {
        if (isAttacking) return blendInput;
        
        Vector3 camR = cameraTransform.right;
        camR.y = 0f; camR.Normalize();

        Vector3 camF = cameraTransform.forward;
        camF.y = 0f; camF.Normalize();

        if (lockOnSystem.IsLocked)
        {
            Vector3 enemyDir = lockOnSystem.CurrentTarget.GetLockTransform().position
                               - transform.position;
            enemyDir.y = 0f; enemyDir.Normalize();

            float eX = Vector3.Dot(enemyDir, camR);
            float eY = Vector3.Dot(enemyDir, camF);
            
            return new Vector2(eX, eY);
        }

        if (targetDirection.magnitude < 0.1f) return Vector2.zero;

        Vector3 d = targetDirection;
        d.y = 0f; d.Normalize();

        return new Vector2(
            Vector3.Dot(d, camR),
            Vector3.Dot(d, camF)
        );
    }

    #endregion

    #region Jump

    private void HandleJump()
    {
        animationManager.Jump();
    }

    #endregion

    #region Shoot

    private void HandleSimpleShoot()
    {
        //animationManager.EndShoot();
    }

    private void HandleChargeShoot(float chargeRatio)
    {
        //animationManager.EndShoot();
    }

    #endregion
}