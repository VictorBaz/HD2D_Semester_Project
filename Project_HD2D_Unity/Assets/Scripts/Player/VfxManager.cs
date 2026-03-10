using System;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRendererDash;

    private void Awake()
    {
        ToggleDashTrail(false);
    }

    public void ToggleDashTrail(bool isOn) => trailRendererDash.enabled = isOn;
}
