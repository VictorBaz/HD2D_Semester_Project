using UnityEngine;

public class EnemySheepManager : EnemyBaseManager
{
    [SerializeField] private GameObject sheepBridgeCollider;
    public SheepBridgeState SheepBridge {get; private set;}
    
    protected override void InitializeState()
    {
        AttackState = new EnemySheepAttackState();
        SheepBridge = new SheepBridgeState();
    }

    protected override void Start()
    {
        base.Start();
        sheepBridgeCollider.SetActive(false);
        context.SetVisualParam(GameConstants.PARAM_SHEEP_SHADER_NAME,0,GameConstants.INDEX_MATERIAL_PULSE);
    }
    

    public void HandleSheepBridgeState()
    {
        switch (CurrentState)
        {
            case SheepBridgeState:
                return;
            case EnemyDropState or EnemyKoState:
                ChangeState(SheepBridge);
                break;
            default:
                ResetEnemy();
                break;
        }


        Debug.Log(CurrentState);
    }

    public void ActivateBridge() => ToggleBridge(true);
    public void DeactivateBridge() => ToggleBridge(false);

    private void ToggleBridge(bool isActive)
    {
        sheepBridgeCollider.SetActive(isActive);
    }
}