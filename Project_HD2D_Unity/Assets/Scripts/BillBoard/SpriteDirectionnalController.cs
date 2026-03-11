using UnityEngine;

public class SpriteDirectionnalController : MonoBehaviour
{
    #region Link
    public Camera cam;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    #endregion

    #region Settings
    [SerializeField] private float backAngleThreshold = 45f;
    [SerializeField] private float sideAngleThreshold = 135f;
    #endregion

    private void LateUpdate()
    {
        
        //TODO : Changer par new input system
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        
        Vector3 moveDirection = (camForward * v + camRight * h).normalized;

        if (moveDirection != Vector3.zero)
        {
            mainTransform.forward = moveDirection;
        }

        float signedAngle = Vector3.SignedAngle(camForward, mainTransform.forward, Vector3.up);
        float angle = Mathf.Abs(signedAngle);

        Vector2 animDirection = Vector2.zero;
        bool flip = false;

        if (angle < backAngleThreshold)
        {
            animDirection = new Vector2(0, -1); 
        }
        else if (angle > sideAngleThreshold)
        {
            animDirection = new Vector2(0, 1);
        }
        else
        {
            animDirection = new Vector2(1, 0);
            flip = (signedAngle < 0);
        }

        animator.SetFloat("moveX", animDirection.x);
        animator.SetFloat("moveY", animDirection.y);
        spriteRenderer.flipX = flip;
    }
}