public class SheepAnimationManager : EnemyAnimationManager
{
    private static readonly int IsBridgeHash = UnityEngine.Animator.StringToHash("isBridge");
    private static readonly int StartBridgeHash = UnityEngine.Animator.StringToHash("StartBridge");

    private void SetBridgeAnim(bool isOn)
    {
        animator.SetBool(IsBridgeHash, isOn);
    }

    public void SetBridgeOn()
    {
        animator.SetTrigger(StartBridgeHash);
        SetBridgeAnim(true);
    }

    public void SetBridgeOff()
    {
        SetBridgeAnim(false);
        animator.ResetTrigger(IsBridgeHash);
    }
    
    
}