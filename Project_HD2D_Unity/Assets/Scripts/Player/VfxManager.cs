using System;
using UnityEngine;
using UnityEngine.VFX;

public class VfxManager : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRendererDash;
    [SerializeField] private VisualEffect linkEffect;

    private void Awake()
    {
        ToggleDashTrail(false);
        ToggleLinkEffect(false);
    }
    

    public void ToggleDashTrail(bool isOn) => trailRendererDash.enabled = isOn;


    private void LinkEffectOn(Transform playerHead, Transform targetTransform)
    {
        linkEffect.gameObject.SetActive(true);
        
        LinkFollow(playerHead, targetTransform);
       
        linkEffect.Play();
    }

    public void LinkFollow(Transform playerHead, Transform targetTransform)
    {
        Vector3 newPosition = Vector3.Lerp(playerHead.position, targetTransform.position, 0.25f);
        Vector3 newPosition2 = Vector3.Lerp(playerHead.position, targetTransform.position, 0.62f);
        
        linkEffect.SetVector3("Position1", playerHead.position);
        linkEffect.SetVector3("Position2", newPosition);
        linkEffect.SetVector3("Position3", newPosition2);
        linkEffect.SetVector3("Position4", targetTransform.position);
        print($" player = {playerHead.position} | target = {targetTransform.position}");
    }

    private void LinkEffectOff()
    {
        linkEffect.gameObject.SetActive(false);
        linkEffect.Stop();
    }
    
    public void ToggleLinkEffect(bool isOn, Transform playerHead = null, Transform targetTransform = null)
    {
        if (!isOn)
        {
            LinkEffectOff();
            return;
        }

        if (playerHead == null || targetTransform == null) return;
        LinkEffectOn(playerHead, targetTransform);
    }
}
