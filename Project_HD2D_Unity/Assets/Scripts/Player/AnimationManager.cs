using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    #region Link
    public Camera cam;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    #endregion

    #region Public Methods

    public void HandleAnimation(float velocity, Vector2 input)
    {
        animator.SetFloat("Velocity", velocity);

        Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        
        Vector3 moveDirection = (camForward * input.x + camRight * input.y).normalized;

        if (moveDirection != Vector3.zero)
        {
            mainTransform.forward = moveDirection;

            animator.SetFloat("moveX", input.x);
            animator.SetFloat("moveY", input.y);
        }
        
    }
    

    #endregion
}