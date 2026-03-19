using UnityEngine;

public class AnimManagerEnnemie : MonoBehaviour
{
    #region Variables
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject colliderAttack;
    
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int IsHitHash = Animator.StringToHash("IsHit");
    private static readonly int IsKOHash = Animator.StringToHash("IsKO");
    #endregion

    #region Movement & States
    
    public void UpdateMovement(float currentSpeed)
    {
        animator.SetFloat(SpeedHash, currentSpeed);
    }

    public void SetGrounded(bool isGrounded)
    {
        animator.SetBool(IsGroundedHash, isGrounded);
    }

    public void SetKO(bool isKO)
    {
        animator.SetBool(IsKOHash, isKO);
    }
    
    #endregion

    #region Combat
    
    public void SetIsHit()
    {
        animator.SetTrigger(IsHitHash);
    }

    public void EnterAttack()
    {
        animator.SetBool(IsAttackingHash, true);
        AttackOn();
    }

    public void ExitAttack()
    {
        animator.SetBool(IsAttackingHash, false);
        AttackOff();
    }

    public bool IsInAttackAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    public bool IsInHitAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit");
    }
    
    #endregion

    #region Colliders & Logic

    private void ToggleAttackCollider(bool toggle)
    {
        if (colliderAttack != null)
            colliderAttack.SetActive(toggle);
    }
    
    public void AttackOn() => ToggleAttackCollider(true);
    public void AttackOff() => ToggleAttackCollider(false);

    public AnimatorStateInfo GetCurrentState(int layer = 0)
    {
        return animator.GetCurrentAnimatorStateInfo(layer);
    }
    #endregion
}