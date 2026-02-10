using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Cible")]
    public Transform target;
    public float distance = 5.0f;

    [Header("Paramètres")]
    public float mouseSensitivity = 100f;
    public float lookXLimit = 85.0f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;
    }

    void LateUpdate() 
    {
        if (!target) return;

        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        
        Vector3 position = target.position - (rotation * Vector3.forward * distance);
        
        transform.rotation = rotation;
        transform.position = position;
    }
}