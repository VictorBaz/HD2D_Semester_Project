using System;
using Manager;
using UnityEngine;

namespace Grid
{
    public class GridObject : MonoBehaviour
    {
        #region Setup Grid Objects

        private void RegisterGridObject()
        {
            EventManager.RegisterObject(
                this,
                GridHelper.WorldToGrid(
                    transform.position,
                    GridSystem.Instance.CellSize));
        }

        #endregion

        #region Unity LifeCycle

        private void Start()
        {
            RegisterGridObject();
        }

        #endregion
    }
}
