using UnityEngine;

public class AnimManagerEnnemie : MonoBehaviour
{
    #region Variables
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject colliderAttack;
    
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int IsHitHash = Animator.StringToHash("IsHit");
    private static readonly int IsKOHash = Animator.StringToHash("IsKO");
    private static readonly int IsExposedHash = Animator.StringToHash("IsExposed");
    #endregion

    #region Movement & States
    
    public void UpdateMovement(float currentSpeed)
    {
        animator.SetFloat(SpeedHash, currentSpeed);
    }

    public void SetFalling(bool isFalling)
    {
        animator.SetBool(IsFallingHash, isFalling);
    }

    public void SetKO(bool isKO)
    {
        animator.SetBool(IsKOHash, isKO);
    }

    public void SetExposed(bool isExposed)
    {
        animator.SetBool(IsExposedHash, isExposed);
    }
    #endregion

    #region Combat
    
    public void SetIsHit(bool isHit)
    {
        animator.SetBool(IsHitHash,isHit);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(IsAttackingHash);
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
        {
            colliderAttack.SetActive(toggle);
        }
        
    }
    
    public void AttackOn() => ToggleAttackCollider(true);
    public void AttackOff() => ToggleAttackCollider(false);

    public AnimatorStateInfo GetCurrentState(int layer = 0)
    {
        return animator.GetCurrentAnimatorStateInfo(layer);
    }
    #endregion
}