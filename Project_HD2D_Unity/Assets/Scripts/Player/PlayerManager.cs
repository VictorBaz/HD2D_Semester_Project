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
        CalculateTargetDirection();
        
        lockOnSystem.UpdateLockRotation();
        
        playerController.SetLockMode(lockOnSystem.IsLocked);
        playerController.UpdatePlayerController(cameraTransform, inputManager.MoveInput);

        Vector2 animationInput = inputManager.MoveInput;//lockOnSystem.IsLocked 
        //    ? CalculateLocalInput(targetDirection) 
         //   : inputManager.MoveInput;
        
        animationManager.HandleAnimation(
            playerController.Rb.linearVelocity.magnitude, 
            animationInput
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
    
    private Vector2 CalculateLocalInput(Vector3 worldDirection)
    {
        if (worldDirection.magnitude < 0.1f) 
            return Vector2.zero;
        
        Vector3 localDirection = playerController.transform.InverseTransformDirection(worldDirection);
        
        Vector2 localInput = new Vector2(localDirection.x, localDirection.z);
        
        if (localInput.magnitude > 1f)
            localInput.Normalize();
        
        return localInput;
    }

    

    #endregion
}