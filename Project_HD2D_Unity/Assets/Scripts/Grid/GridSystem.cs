using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Grid
{
    public class GridSystem : MonoBehaviour
    {
        #region Variables

        Dictionary<Vector3Int, GridObject> dictionnaryGridObjects = new Dictionary<Vector3Int, GridObject>();
        
        [SerializeField] private float cellSize = 1f;

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
                throw new  Exception("A GridObject is already placed here");
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
        

        public  Vector3Int WorldToGrid(Vector3 worldPosition)
        {
            Vector3Int gridPosition = new Vector3Int
            (
                Mathf.FloorToInt(worldPosition.x / cellSize),
                Mathf.FloorToInt(worldPosition.y / cellSize),
                Mathf.FloorToInt(worldPosition.z / cellSize)
            );
            
            return gridPosition;
        }

        public  Vector3 GridToWorld(Vector3Int gridPosition)
        {
            Vector3 wordPosition = new Vector3
            (
                (gridPosition.x * cellSize) + (cellSize * 0.5f),
                (gridPosition.y * cellSize) /* + (cellSize * 0.5f)*/ ,
                (gridPosition.z * cellSize) + (cellSize * 0.5f)
            );
            
            return wordPosition;
        }

        public bool TryGetGridObjectAt(Vector3Int gridPosition, out GridObject gridObject)
        {
            return dictionnaryGridObjects.TryGetValue(gridPosition, out gridObject);
        }

        public  bool IsPositionOccupied(Vector3Int gridPosition)
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
