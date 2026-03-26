using UnityEngine;

public abstract class BaseAnimationManager : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    protected static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    protected static readonly int IsHitHash = Animator.StringToHash("IsHit");
    protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    protected static readonly int JumpTriggerHash = Animator.StringToHash("Jump"); // Centralisé ici

    public virtual void SetFalling(bool isFalling) => animator.SetBool(IsFallingHash, isFalling);
    public virtual void SetHit(bool isHit) => animator.SetBool(IsHitHash, isHit);
    
    public virtual void TriggerJump() => animator.SetTrigger(JumpTriggerHash);
    
    public bool IsInAttackAnimation() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    public bool IsInHitAnimation() => animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit");

    public AnimatorStateInfo GetCurrentState(int layer = 0) => animator.GetCurrentAnimatorStateInfo(layer);
}