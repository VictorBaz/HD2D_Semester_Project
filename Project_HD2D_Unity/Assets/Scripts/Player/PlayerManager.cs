using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AnimationManager animationManager;
    [SerializeField] private Transform cameraTransform;
    
    private Vector3 targetDirection;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        inputManager.OnJumpPressed += playerController.TryJump;
    }

    private void OnDisable()
    {
        inputManager.OnJumpPressed -= playerController.TryJump;
    }

    private void Update()
    {
        CalculateTargetDirection();
        playerController.UpdatePlayerController(cameraTransform, inputManager.MoveInput);
        animationManager.HandleAnimation(playerController.Rb.linearVelocity.magnitude, inputManager.MoveInput);
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

    #endregion
}