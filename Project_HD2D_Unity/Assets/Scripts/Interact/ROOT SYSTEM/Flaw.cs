using System;
using UnityEditor;
using UnityEngine;

public class Flaw : MonoBehaviour, IEnergyLockable, IRootLink
{
    #region Variables

    private Root root;
    
    [SerializeField] private Transform pivotPoint;

    #endregion
    
    #region IEnergyLockable

    public Transform GetLockTransform() => pivotPoint;
    public bool IsLockable() => true;
    public float GetLockPriority() => 1f;

    public bool IsContainingEnergy() => root.IsContainingEnergy();
    public bool IsAtMaximumEnergy() => root.IsAtMaximumEnergy();

    public void AddEnergy() => root.AddEnergy();
    public void RemoveEnergy() => root.RemoveEnergy();

    #endregion

    #region Gizmos

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle
        {
            normal =
            {
                textColor = Color.white
            },
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        Handles.Label(transform.position,"ROOT",style);
    }
#endif
    

    #endregion

    #region Init

    public void SetRoot(Root root)
    {
        this.root = root;
    }

    #endregion
    
}