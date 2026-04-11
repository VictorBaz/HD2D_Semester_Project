using UnityEngine;

public class Sap : MonoBehaviour, ISapLockable
{
    #region Variables
    private bool      _isEmpty;
    private Transform _playerTransform;
    #endregion
    

    #region ISapLockable
    public Transform GetLockTransform() => transform;

    public bool IsLockable() => !_isEmpty;

    public float GetLockPriority() => 1f;

    public void GiveSap()
    {
        _isEmpty = true;
    }
    #endregion
}