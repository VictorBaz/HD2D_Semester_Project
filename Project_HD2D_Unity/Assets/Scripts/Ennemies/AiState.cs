using UnityEngine;

public abstract class AiState
{
    #region State Methods
    public abstract void EnterState(AiContext actx);
    public abstract void UpdateState(AiContext actx);
    public abstract void ExitState(AiContext actx);

    public virtual string Name { get; private set; }
    
    public virtual bool CanAttack => false;
    public virtual bool CanMove => true;
    public virtual bool CanTakeDamage => true;
    public virtual bool IsFalling => false;
    

    #endregion
}