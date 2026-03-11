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
        //pour perpandiculaire après transform.Look at transform.rotation.y = 0 par rapport X
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                transform.LookAt(cameraTransform.position, Vector3.up);
                break;
            case BillboardType.CameraForward:
                transform.forward = cameraTransform.forward;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Vector3 rotation = transform.rotation.eulerAngles;
        
        transform.rotation = Quaternion.Euler(0, rotation.y, rotation.z);
        
        if (lockX) { rotation.x = originalRotation.x; }
        if (lockY) { rotation.y = originalRotation.y; }
        if (lockZ) { rotation.z = originalRotation.z; }
    }

    #endregion
    
}
