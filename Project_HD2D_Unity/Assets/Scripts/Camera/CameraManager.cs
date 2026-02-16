using System;
using System.Collections;
using Enum;
using Manager;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    [Header("Components")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;
    
    [Header("Settings Camera Fix")]
    [SerializeField] private float travelDuration = 1f;
    
    [Header("Settings Camera Follow")]
    [SerializeField] private Vector3 offsetCamera;
    [SerializeField] private float smoothTime = 0.3f;
    
    private CameraPlayerState cameraState = CameraPlayerState.Fix;
    private float cameraPositionY = 0f;
    
    private Coroutine cameraCoroutine;

    private void Awake()
    {
        if (cameraTransform == null)
        {
            Debug.LogError($"{nameof(CameraManager)} : CameraTransform non assigné !");
            enabled = false;
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
    

    private void FixedUpdate()
    {
        FollowPlayer();
    }

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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
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
            float ratio = elapsedTime / travelDuration;

            /*float x = Mathf.Lerp(startPosition.x, cameraSettings.CameraPosition.x, ratio);
            float z = Mathf.Lerp(startPosition.z, cameraSettings.CameraPosition.y, ratio);*/
            Vector3 newPosition = Vector3.Lerp(startPosition, cameraSettings.CameraPosition, ratio);


            cameraTransform.position = newPosition;
            yield return null;
        }

        cameraTransform.position = cameraSettings.CameraPosition;
        
        cameraCoroutine = null;
    }

    private void FollowPlayer()
    {
        if (cameraState != CameraPlayerState.FollowPlayer) return;
        
        Vector3 targetPosition = playerTransform.position +  offsetCamera;
        
        Vector3 newPosition = Vector3.Lerp(
            cameraTransform.position,
            targetPosition,
            Time.deltaTime * smoothTime
            );
        
        
        newPosition = new Vector3(newPosition.x, cameraPositionY, newPosition.z);
        
        cameraTransform.position = newPosition;
    }
}