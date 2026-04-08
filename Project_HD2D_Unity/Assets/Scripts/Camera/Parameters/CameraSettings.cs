using System;
using Enum;
using UnityEngine;

[System.Serializable]
public class CameraSettings
{
    public Vector3 CameraPosition;
    public CameraPlayerState CameraPlayerState;
    public float holdDuration;
    
    public Rail ActiveRail; 
}