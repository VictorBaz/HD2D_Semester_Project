using UnityEngine;

public class ResetBoolEnterAnim : StateMachineBehaviour
{
    [SerializeField] private string boolNameEnter;
    [SerializeField] private bool boolValueEnter;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(boolNameEnter, boolValueEnter);
    }
}
