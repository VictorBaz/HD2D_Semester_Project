using System;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class VfxManager : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRendererDash;
    /*[SerializeField] private VisualEffect linkEffect;*/

    /*[SerializeField] private Transform[] linkPoints;*/

    private void Awake()
    {
        ToggleDashTrail(false);
        /*ToggleLinkEffect(false);*/
    }
    

    public void ToggleDashTrail(bool isOn) => trailRendererDash.enabled = isOn;


    /*private void LinkEffectOn(Transform playerHead, Transform targetTransform)
    {
        linkEffect.gameObject.SetActive(true);
        
        LinkFollow(playerHead, targetTransform);
       
        linkEffect.Play();
    }

    public void LinkFollow(Transform playerHead, Transform targetTransform)
    {
        Vector3 newPosition = Vector3.Lerp(playerHead.position, targetTransform.position, 0.25f);
        newPosition.y = playerHead.position.y * 1.3f;
        Vector3 newPosition2 = Vector3.Lerp(playerHead.position, targetTransform.position, 0.62f);
        newPosition2.y = playerHead.position.y * 1.3f;
        
        linkPoints[0].position = playerHead.position;
        linkPoints[1].position = newPosition;
        linkPoints[2].position = newPosition2;
        linkPoints[3].position = targetTransform.position;
       
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
    }*/
}
