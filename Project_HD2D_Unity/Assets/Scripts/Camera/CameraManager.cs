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
    private float cameraPositionY = 0f;
    
    private Coroutine cameraCoroutine;
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
    }

    private void OnDisable()
    {
        EventManager.OnCameraTrigger -= OnCameraTrigger;
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

    /*
    #region Camera Collision

    private void HandleCameraCollisions()
    {
        float targetPosition = m_defaultPosition;
        RaycastHit hit;
        Vector3 direction = m_cameraTransform.position - m_cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(m_cameraPivot.position, m_cameraCollisionRadius,direction, out hit, Mathf.Abs(targetPosition),m_collisionLayer))
        {
            float distance = Vector3.Distance(m_cameraPivot.position, hit.point);
            targetPosition =- (distance - m_cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < m_minimumCollisionOffSet)
        {
            targetPosition -= m_minimumCollisionOffSet;
        }

        m_cameraVectorPosition.z = Mathf.Lerp(m_cameraTransform.localPosition.z, targetPosition, 0.2f);
        m_cameraTransform.localPosition = m_cameraVectorPosition;
    }

    #endregion*/
    
}