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

    public void HandleAnimation(float velocity)
    {
        animator.SetFloat("Velocity", velocity);
    }
    
    private void LateUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        
        Vector3 moveDirection = (camForward * v + camRight * h).normalized;

        if (moveDirection != Vector3.zero)
        {
            mainTransform.forward = moveDirection;

            animator.SetFloat("moveX", h);
            animator.SetFloat("moveY", v);
        }
    }

    #endregion
}