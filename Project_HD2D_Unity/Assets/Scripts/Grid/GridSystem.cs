using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Grid
{
    public class GridSystem : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Dictionary<Vector3Int, GridObject> dictionnaryGridObjects = new Dictionary<Vector3Int, GridObject>();
        
        [SerializeField] private float cellSize = 1f;

        #endregion

        #region Singleton

        public static GridSystem Instance { get; private set; }

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            SetupSub();
        }

        private void OnDisable()
        {
            CleanSub();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Setup & Clean Methods

        private void SetupSub()
        {
            EventManager.OnObjectRegister += RegisterObject;
            EventManager.OnObjectUnregister += UnregisterObject;
            EventManager.OnObjectMoved += MoveObject;
        }

        private void CleanSub()
        {
            EventManager.OnObjectRegister -= RegisterObject;
            EventManager.OnObjectUnregister -= UnregisterObject;
            EventManager.OnObjectMoved -= MoveObject;
        }

        #endregion

        #region Grid Methods

        private void RegisterObject(GridObject gridObject, Vector3Int position)
        {
            if (!dictionnaryGridObjects.TryAdd(position, gridObject))
            {
                throw new  Exception($"A GridObject is already placed here at {position}");
            }
        }

        private void UnregisterObject(GridObject gridObject, Vector3Int position)
        {
            dictionnaryGridObjects.Remove(position);
        }

        private void MoveObject(GridObject gridObject, Vector3Int fromPosition, Vector3Int toPosition)
        {
            if (!dictionnaryGridObjects.ContainsKey(fromPosition)) return;
            if (IsPositionOccupied(toPosition)) return;
    
            dictionnaryGridObjects.Remove(fromPosition);
            dictionnaryGridObjects.Add(toPosition, gridObject);
        }
        

        public bool TryGetGridObjectAt(Vector3Int gridPosition, out GridObject gridObject)
        {
            return dictionnaryGridObjects.TryGetValue(gridPosition, out gridObject);
        }

        public bool IsPositionOccupied(Vector3Int gridPosition)
        {
            return dictionnaryGridObjects.ContainsKey(gridPosition);
        }
        
        #endregion

        #region Properties 

        public float CellSize => cellSize;
        public int ObjectCount => dictionnaryGridObjects.Count;

        #endregion
    }
}
