using UnityEngine;

[System.Serializable]
public abstract class AiState
{
    #region State Methods
    public abstract void EnterState(AiBehavior aiBehavior);
    public abstract void UpdateState(AiBehavior aiBehavior);
    public abstract void ExitState(AiBehavior aiBehavior);

    public virtual string Name { get; private set; }

    #endregion
}