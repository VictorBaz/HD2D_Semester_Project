using System.Collections;
using Manager;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float travelDuration = 1f;

    private Coroutine cameraCoroutine;

    private void Awake()
    {
        if (cameraTransform == null)
        {
            Debug.LogError($"{nameof(CameraManager)} : CameraTransform non assigné !");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        EventManager.OnCameraTrigger += TravelingCamera;
    }

    private void OnDisable()
    {
        EventManager.OnCameraTrigger -= TravelingCamera;
    }

    private void TravelingCamera(Vector3 newPosition, Vector3 newRotation)
    {
        if (cameraCoroutine != null)
            StopCoroutine(cameraCoroutine);

        cameraCoroutine = StartCoroutine(TravelingCameraIE(newPosition, newRotation));
    }

    private IEnumerator TravelingCameraIE(Vector3 targetPosition, Vector3 targetEulerRotation)
    {
        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetEulerRotation);

        float elapsedTime = 0f;

        while (elapsedTime < travelDuration)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / travelDuration;

            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, ratio);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, ratio);

            yield return null;
        }

        cameraTransform.position = targetPosition;
        cameraTransform.rotation = targetRotation;
        cameraCoroutine = null;
    }
}