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
    
    public static Func<bool> OnRequestIsPlayerLock;
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
    
    #region Game Flow
    public static event Action<GameState> OnGameStateChanged;
    public static void TriggerGameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);
    #endregion
    
    #region UI & HUD
    public static event Action<bool> OnToggleInputPanel;
    public static void TriggerToggleInputPanel(bool show) => OnToggleInputPanel?.Invoke(show);

    public static event Action<int, int> OnEnergyChanged;
    public static void TriggerEnergyChanged(int current, int max) => OnEnergyChanged?.Invoke(current, max);

    public static event Action<int, int> OnSapChanged;
    public static void TriggerSapChanged(int current, int max) => OnSapChanged?.Invoke(current, max);
    
    public static event Action<bool> OnLockStateChanged;
    public static void TriggerLockStateChanged(bool isLocked) => OnLockStateChanged?.Invoke(isLocked);
    #endregion
    
    public static event Action OnLoadingStarted;
    public static void TriggerLoadingStarted() => OnLoadingStarted?.Invoke();
}