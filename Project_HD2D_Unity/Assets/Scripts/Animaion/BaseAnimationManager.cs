using UnityEngine;

public abstract class BaseAnimationManager : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    protected static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    protected static readonly int IsHitHash = Animator.StringToHash("IsHit");
    protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

    public virtual void SetFalling(bool isFalling) => animator.SetBool(IsFallingHash, isFalling);
    public virtual void SetHit(bool isHit) => animator.SetBool(IsHitHash, isHit);
    
    public bool IsInAttackAnimation() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    public bool IsInHitAnimation() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit");

    public AnimatorStateInfo GetCurrentState(int layer = 0) => animator.GetCurrentAnimatorStateInfo(layer);
}