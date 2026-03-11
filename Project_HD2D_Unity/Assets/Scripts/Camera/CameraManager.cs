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
    
    [Header("Settings Camera Traveling")]
    [SerializeField] private float travelDuration = 1f;
    
    [Header("Settings Camera Follow")]
    [SerializeField] private Vector3 offsetCamera;
    [SerializeField] private float smoothTime = 0.3f;
    
    [SerializeField] private CameraPlayerState cameraState = CameraPlayerState.Fix;
    [SerializeField] private Transform childTransform;
    
    
    private float cameraPositionY = 0f;
    
    private Coroutine cameraCoroutine;
    private Coroutine shakeCoroutine;
    
    private Vector3 velocity = Vector3.zero;

    #endregion
    
    #region Unity Lifecycle

    private void Awake()
    {
        if (cameraTransform == null)
        {
            Debug.LogError($"{nameof(CameraManager)} : CameraTransform non assigne !");
            enabled = false;
            return;
        }
        
        cameraPositionY = cameraTransform.position.y;
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
        FollowPlayer();
    }

    #endregion

    #region Camera Trigger

    private void OnCameraTrigger(CameraSettings cameraSettings)
{
    switch (cameraSettings.CameraPlayerState)
    {
        case CameraPlayerState.Fix:
            cameraState = CameraPlayerState.Fix;
            TravelingCamera(cameraSettings);
            break;
        case CameraPlayerState.FollowPlayer:
            cameraState = CameraPlayerState.FollowPlayer;
            break;
        case CameraPlayerState.Cinematic:
            cameraState = CameraPlayerState.Cinematic;
            if (cameraCoroutine != null) StopCoroutine(cameraCoroutine);
            cameraCoroutine = StartCoroutine(CinematicCameraIE(cameraSettings));
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
}

    #endregion

    #region Traveling

    private void TravelingCamera(CameraSettings cameraSettings)
    {
        if (cameraCoroutine != null)
            StopCoroutine(cameraCoroutine);

        cameraCoroutine = StartCoroutine(TravelingCameraIE(cameraSettings));
    }

    private IEnumerator TravelingCameraIE(CameraSettings cameraSettings)
    {
        Vector3 startPosition = cameraTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < travelDuration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = Mathf.SmoothStep(0f, 1f, elapsedTime / travelDuration);

            Vector3 newPosition = Vector3.Lerp(startPosition, cameraSettings.CameraPosition, ratio);
            cameraTransform.position = newPosition;
            
            yield return null;
        }

        Vector3 finalPosition = cameraSettings.CameraPosition;
        cameraTransform.position = finalPosition;
        
        cameraCoroutine = null;
    }

    #endregion

    #region Follow Player

    private void FollowPlayer()
    {
        if (cameraState != CameraPlayerState.FollowPlayer) return;
        
        if (cameraCoroutine != null)
            StopCoroutine(cameraCoroutine);

        
        Vector3 targetPosition = playerTransform.position + offsetCamera;
        
        Vector3 newPosition = Vector3.SmoothDamp(
            cameraTransform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
        
        cameraTransform.position = newPosition;
    }

    #endregion

    #region Cinematic

    private IEnumerator CinematicCameraIE(CameraSettings cameraSettings)
    {
        yield return StartCoroutine(TravelingCameraIE(cameraSettings));
        
        yield return new WaitForSeconds(cameraSettings.holdDuration);
        
        CameraSettings returnSettings = new CameraSettings
        {
            CameraPosition = playerTransform.position + offsetCamera,
            CameraPlayerState = CameraPlayerState.FollowPlayer
        };
        
        yield return StartCoroutine(TravelingCameraIE(returnSettings));
        
        cameraState = CameraPlayerState.FollowPlayer;
        cameraCoroutine = null;
    }

    #endregion

    #region Shake

    private void Shake()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeIE(0.2f,0.7f));
    }
    
    private IEnumerator ShakeIE(float duration, float magnitude)
    {
        float elapsed = 0f;
        float seed    = UnityEngine.Random.value * 100f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float currentMagnitude = Mathf.Lerp(magnitude, 0f, t);

            childTransform.localPosition = new Vector3(
                (Mathf.PerlinNoise(seed + t * 10f, 0f) - 0.5f) * 2f,
                0,
                (Mathf.PerlinNoise(0f, seed + t * 10f) - 0.5f) * 2f) * currentMagnitude;

            yield return null;
        }

        childTransform.localPosition = Vector3.zero;
        shakeCoroutine = null;
    }

    #endregion
}