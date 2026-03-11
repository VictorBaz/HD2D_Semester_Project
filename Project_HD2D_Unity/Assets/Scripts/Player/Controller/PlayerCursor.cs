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
    

    private Quaternion targetRotation;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        targetRotation = transform.rotation;
    }

    #endregion

    #region Public Methods
    public void HandleRotation(Vector3 worldDirection)
    {
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