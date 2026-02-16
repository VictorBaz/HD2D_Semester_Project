using System;
using Enum;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CameraSettings
{
    public Vector3 CameraPosition;
    public float CameraSize;
    public float CameraFOV;
    public CameraPlayerState CameraPlayerState;
}
