using UnityEngine;

public class ResetBoolExitAnim : StateMachineBehaviour
{
    [SerializeField] private string boolNameExit;
    [SerializeField] private bool boolValueExit;
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        animator.SetBool(boolNameExit, boolValueExit);
    }
}
