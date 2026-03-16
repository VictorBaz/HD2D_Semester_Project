using UnityEngine;

public abstract class AiState
{
    #region State Methods
    public abstract void EnterState(AiContext actx);
    public abstract void UpdateState(AiContext actx);
    public abstract void ExitState(AiContext actx);

    public virtual string Name { get; private set; }

    #endregion
}