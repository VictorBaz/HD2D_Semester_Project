using System;
using Enum;
using UnityEngine;

[Serializable]
public class CameraSettings
{
    public Vector3 CameraPosition;
    public CameraPlayerState CameraPlayerState;
    public float holdDuration;
}