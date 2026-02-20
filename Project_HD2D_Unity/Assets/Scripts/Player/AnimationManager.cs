using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    #region Variables
    public Camera cam;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private static readonly int VelocityHash = Animator.StringToHash("Velocity");
    private static readonly int MoveXHash = Animator.StringToHash("moveX");
    private static readonly int MoveYHash = Animator.StringToHash("moveY");
    
    #endregion

    #region Public Methods

    public void HandleAnimation(float velocity, Vector2 input)
    {
        animator.SetFloat(VelocityHash, velocity, 0.05f, Time.deltaTime);

        Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        
        Vector3 moveDirection = (camForward * input.x + camRight * input.y).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            mainTransform.forward = moveDirection;
            
            animator.SetFloat(MoveXHash, input.x, 0.1f, Time.deltaTime);
            animator.SetFloat(MoveYHash, input.y, 0.1f, Time.deltaTime);
        }
        
    }
    
    #endregion
}