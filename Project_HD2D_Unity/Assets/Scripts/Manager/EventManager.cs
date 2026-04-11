using System;
using Grid;
using UnityEngine;

public static class EventManager
{
    #region Player & Context
    /// <summary> Requête pour obtenir le Transform du joueur </summary>
    public static Func<Transform> OnRequestPlayerTransform;
        
    /// <summary> Requête pour obtenir le contexte d'état du joueur </summary>
    public static Func<PlayerStateContext> OnRequestPlayerContext;
    #endregion

    #region Grid Systems
    public static event Action<GridObject, Vector3Int> OnObjectRegister;
    public static event Action<GridObject, Vector3Int> OnObjectUnregister;
    /// <summary> GridObject, FromPosition, ToPosition </summary>
    public static event Action<GridObject, Vector3Int, Vector3Int> OnObjectMoved;

    public static void RegisterObject(GridObject gridObject, Vector3Int position) => OnObjectRegister?.Invoke(gridObject, position);
    public static void UnregisterObject(GridObject gridObject, Vector3Int position) => OnObjectUnregister?.Invoke(gridObject, position);
    public static void MovedObject(GridObject gridObject, Vector3Int from, Vector3Int to) => OnObjectMoved?.Invoke(gridObject, from, to);
    #endregion

    #region Camera & Feedback
    public static event Action<CameraSettings> OnCameraTrigger;
    public static event Action OnCameraShake;

    public static void TriggerCamera(CameraSettings settings) => OnCameraTrigger?.Invoke(settings);
    public static void CameraShake() => OnCameraShake?.Invoke();
    #endregion

    #region Gameplay & Puzzles
    /// <summary> Appelé quand un boss de zone est vaincu. Paramètre : ID du puzzle </summary>
    public static event Action<string> OnPuzzleCompleted;

    public static void TriggerPuzzleCompleted(string puzzleID) => OnPuzzleCompleted?.Invoke(puzzleID);
    #endregion
}