using System;
using Grid;
using UnityEngine;

public static class GridEvents
{
    public static event Action<GridObject, Vector3Int> OnObjectRegister;
    public static event Action<GridObject, Vector3Int> OnObjectUnregister;
    /// <summary> GridObject, FromPosition, ToPosition </summary>
    public static event Action<GridObject, Vector3Int, Vector3Int> OnObjectMoved;

    public static void RegisterObject(GridObject gridObject, Vector3Int position) => OnObjectRegister?.Invoke(gridObject, position);
    public static void UnregisterObject(GridObject gridObject, Vector3Int position) => OnObjectUnregister?.Invoke(gridObject, position);
    public static void MovedObject(GridObject gridObject, Vector3Int from, Vector3Int to) => OnObjectMoved?.Invoke(gridObject, from, to);
}