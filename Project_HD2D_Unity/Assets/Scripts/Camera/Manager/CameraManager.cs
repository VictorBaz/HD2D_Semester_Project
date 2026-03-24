using System;
using System.Collections;
using Enum;
using Manager;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Variables
    [Header("Components")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform childTransform; 

    [Header("Settings Camera Follow")]
    [SerializeField] private Vector3 offsetCamera;
    [SerializeField] private float smoothTimeFix = 0.3f;
    [SerializeField] private float smoothTimeFollow = 0.3f;
    [SerializeField] private float smoothTimeRail = 0.3f;

    [Header("Settings Camera Traveling")]
    [SerializeField] private float travelDuration = 1f;
    
    [Header("Collision Settings")]
    [SerializeField] private LayerMask CollisionLayers; 
    [SerializeField] private  float CollisionPadding = 0.2f;

    public CameraFollowState FollowState { get; private set; }
    public CameraFixState FixState { get; private set; }
    public CameraCinematicState CinematicState { get; private set; }
    public CameraRailState RailState { get; private set; }

    private CameraBaseState currentState;
    private CameraStateContext context;
    private Coroutine shakeCoroutine;

    public float TravelDuration => travelDuration;
    
    [SerializeField] private CameraPlayerState startingState;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (cameraTransform == null || playerTransform == null)
        {
            enabled = false;
            return;
        }

        FollowState = new CameraFollowState();
        FixState = new CameraFixState();
        CinematicState = new CameraCinematicState();
        RailState = new CameraRailState();

        context = new CameraStateContext
        {
            Manager = this,
            CameraTransform = cameraTransform,
            PlayerTransform = playerTransform,
            Offset = offsetCamera,
            SmoothTimeFix = smoothTimeFix,
            SmoothTimeFollow = smoothTimeFollow,
            SmoothTimeRail = smoothTimeRail,
            CollisionLayers = this.CollisionLayers,
            CollisionPadding = this.CollisionPadding,
        };

        TransitionTo(ConvertEnumToState(startingState));
    }

    private void OnEnable()
    {
        EventManager.OnCameraTrigger += OnCameraTrigger;
        EventManager.OnCameraShake += Shake;
    }

    private void OnDisable()
    {
        EventManager.OnCameraTrigger -= OnCameraTrigger;
        EventManager.OnCameraShake -= Shake;
    }

    private void LateUpdate()
    {
        if (currentState != null)
        {
            currentState.UpdateState(context);
        }
    }
    #endregion

    #region State Management
    public void TransitionTo(CameraBaseState newState)
    {
        if (newState == null) return;

        if (currentState != null)
            currentState.ExitState(context);

        currentState = newState;
        
        context.Velocity = Vector3.zero; 

        currentState.EnterState(context);
    }

    private void OnCameraTrigger(CameraSettings settings)
    {
        context.CurrentSettings = settings;

        switch (settings.CameraPlayerState)
        {
            case CameraPlayerState.FollowPlayer:
                TransitionTo(FollowState);
                break;
            case CameraPlayerState.Fix:
                TransitionTo(FixState);
                break;
            case CameraPlayerState.Cinematic:
                TransitionTo(CinematicState);
                break;
            case CameraPlayerState.Rail: 
                TransitionTo(RailState);
                break;
        }
    }
    #endregion

    #region Shake Logic
    private void Shake()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeIE(0.2f, 0.7f));
    }

    private IEnumerator ShakeIE(float duration, float magnitude)
    {
        float elapsed = 0f;
        float seed = UnityEngine.Random.value * 100f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float currentMagnitude = Mathf.Lerp(magnitude, 0f, t);

            childTransform.localPosition = new Vector3(
                (Mathf.PerlinNoise(seed + t * 10f, 0f) - 0.5f) * 2f * currentMagnitude,
                (Mathf.PerlinNoise(0f, seed + t * 10f) - 0.5f) * 2f * currentMagnitude,
                0
            );

            yield return null;
        }

        childTransform.localPosition = Vector3.zero;
        shakeCoroutine = null;
    }
    #endregion
    
    
    private void OnDrawGizmos()
    {
        if (context == null || cameraTransform == null || playerTransform == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(cameraTransform.position, playerTransform.position + Vector3.up * 1.5f);

        if (currentState == FollowState)
        {
            Gizmos.color = Color.cyan;
            Vector3 targetPos = playerTransform.position + offsetCamera;
            Gizmos.DrawWireSphere(targetPos, 0.5f);
            Gizmos.DrawLine(cameraTransform.position, targetPos);
        }
        else if (currentState == FixState || currentState == CinematicState)
        {
            Gizmos.color = Color.red;
            if (context.CurrentSettings != null)
            {
                Gizmos.DrawWireCube(context.CurrentSettings.CameraPosition, Vector3.one * 0.5f);
                Gizmos.DrawLine(cameraTransform.position, context.CurrentSettings.CameraPosition);
            }
        }
        else if (currentState == RailState)
        {
            Gizmos.color = Color.magenta;
            if (context.CurrentSettings != null && context.CurrentSettings.ActiveRail != null)
            {
                Vector3 railPoint = context.CurrentSettings.ActiveRail.ProjectPositionOnRail(playerTransform.position);
                Gizmos.DrawWireSphere(railPoint, 0.3f);
            }
        }
    }

    private CameraBaseState ConvertEnumToState(CameraPlayerState state)
    {
        switch (state)
        {
            case CameraPlayerState.Fix:
                return FixState;
            case CameraPlayerState.FollowPlayer:
                return FollowState;
            case CameraPlayerState.Cinematic:
                return CinematicState;
            case CameraPlayerState.Rail:
                return RailState;
            default:
                return FollowState;
        }
        
    }
    
}