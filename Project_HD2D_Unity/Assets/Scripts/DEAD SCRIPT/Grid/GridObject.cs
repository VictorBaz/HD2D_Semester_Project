using System;
using Manager;
using UnityEngine;

namespace Grid
{
    public class GridObject : MonoBehaviour
    {
        #region Variables

        private Vector3Int currentGridPosition;

        #endregion
        
        #region Setup Grid Objects

        private void RegisterGridObject()
        {
            EventManager.RegisterObject(
                this,
                GridHelper.WorldToGrid(
                    transform.position,
                    GridSystem.Instance.CellSize));
        }
        
        private void UnregisterGridObject()
        {
            EventManager.UnregisterObject(this, currentGridPosition);
        }

        #endregion

        #region Unity LifeCycle

        private void Start()
        {
            RegisterGridObject();
        }
        
        private void OnDestroy()
        {
            UnregisterGridObject();
        }

        #endregion
    }
}
