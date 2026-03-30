using System;
using Enum;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    #region Variables

    [SerializeField] private BillboardType billboardType;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    
    private Vector3 originalRotation;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        originalRotation = cameraTransform.rotation.eulerAngles;
    }
    
    private void LateUpdate()
    {
        DisplaySpriteBillboard();
    }

    #endregion

    #region Methods

    private void DisplaySpriteBillboard()
    {
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                transform.LookAt(cameraTransform.position, Vector3.up);
                break;
            case BillboardType.CameraForward:
                transform.forward = cameraTransform.forward;
                break;
        }

        Vector3 currentRotation = transform.rotation.eulerAngles;
        
        float x = lockX ? 0f : currentRotation.x;
        float y = lockY ? 0f : currentRotation.y;
        float z = lockZ ? 0f : currentRotation.z;

        transform.rotation = Quaternion.Euler(x, y, z);
    }

    #endregion
    
}
