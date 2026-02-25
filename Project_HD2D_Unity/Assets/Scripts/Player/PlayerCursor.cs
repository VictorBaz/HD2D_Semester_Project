using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;

    [Header("Settings")]
    [Tooltip("Vertical offset so the cursor sits at the player's feet")]
    [SerializeField] private float yOffset = 0.05f;

    [Tooltip("Minimum stick magnitude to update rotation (deadzone)")]
    [SerializeField] private float inputDeadzone = 0.15f;

    [Tooltip("How fast the cursor snaps to the new direction")]
    [SerializeField] private float rotationSpeed = 20f;

    private Quaternion targetRotation;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        targetRotation = transform.rotation;
    }

    #endregion

    #region Public Methods
    public void HandleRotation(Vector2 shootInput)
    {
        

        if (shootInput.magnitude < inputDeadzone) return;

        Vector3 camRight   = cameraTransform.right;
        camRight.y         = 0f;
        camRight.Normalize();

        Vector3 camForward  = cameraTransform.forward;
        camForward.y        = 0f;
        camForward.Normalize();

        Vector3 worldDirection = camRight * shootInput.x + camForward * shootInput.y;
        worldDirection.y = 0f;

        if (worldDirection.sqrMagnitude < 0.001f) return;

        targetRotation = Quaternion.LookRotation(worldDirection);

        transform.rotation = targetRotation;
    }

    #endregion

    #region Private Methods

    public void FollowPlayer()
    {
        transform.position = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y - yOffset,  
            playerTransform.position.z);
    }

    #endregion
}