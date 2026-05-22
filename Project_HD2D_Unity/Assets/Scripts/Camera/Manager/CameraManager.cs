using System;
using Enum;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform childTransform; 

    [Header("Global Settings")]
    [SerializeField] private Vector3 offsetCamera;
    [SerializeField] private LayerMask collisionLayers; 
    [SerializeField] private float collisionPadding = 0.2f;
    [SerializeField] private CameraPlayerState startingState;
    [SerializeField] private float rotationSmoothTime = 0.15f;

    public CameraFollowState FollowState { get; } = new();
    public CameraFixState FixState { get; } = new();
    public CameraCinematicState CinematicState { get; } = new();
    public CameraRailState RailState { get; } = new();

    private CameraBaseState currentState;
    private CameraStateContext context;
    
    private CameraBaseState stateBeforeCinematic;
    private CameraSettings settingsBeforeCinematic;

    private void Awake()
    {
        context = new CameraStateContext
        {
            Manager = this,
            CameraTransform = cameraTransform,
            PlayerTransform = playerTransform,
            Offset = offsetCamera,
            CollisionLayers = collisionLayers,
            CollisionPadding = collisionPadding
        };

        TransitionTo(ConvertEnumToState(startingState), null);
    }

    private void OnEnable() => CameraEvents.OnCameraTrigger += OnCameraTrigger;
    private void OnDisable() => CameraEvents.OnCameraTrigger -= OnCameraTrigger;

    private void LateUpdate() => currentState?.UpdateState(context);

    public void TransitionTo(CameraBaseState newState, CameraSettings newSettings)
    {
        if (newState == null) return;

        if (newState == CinematicState)
        {
            stateBeforeCinematic = currentState;
            settingsBeforeCinematic = context.CurrentSettings; 
        }

        context.CurrentSettings = newSettings;
        context.TransitionSpeed = newSettings?.transitionSmoothTime ?? 0.3f;

        currentState?.ExitState(context);
        currentState = newState;
        currentState.EnterState(context);
    }

    private void OnCameraTrigger(CameraSettings settings)
    {
        TransitionTo(ConvertEnumToState(settings.CameraPlayerState), settings);
    }

    private CameraBaseState ConvertEnumToState(CameraPlayerState state) => state switch
    {
        CameraPlayerState.Fix => FixState,
        CameraPlayerState.Cinematic => CinematicState,
        CameraPlayerState.Rail => RailState,
        _ => FollowState
    };

    public void ReturnFromCinematic()
    {
        if (stateBeforeCinematic == null || settingsBeforeCinematic == null)
        {
            TransitionTo(FollowState, null); 
            return;
        }
        
        context.Velocity = Vector3.zero; 

        TransitionTo(stateBeforeCinematic, settingsBeforeCinematic);
        
        stateBeforeCinematic = null;
        settingsBeforeCinematic = null;
    }
    
    public float RotationSmoothTime => rotationSmoothTime;
}