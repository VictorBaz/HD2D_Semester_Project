using UnityEngine;
using DG.Tweening; 

public class WaterDrop : MonoBehaviour
{
    [SerializeField] private float fallDuration = 1f;
    [SerializeField] private float fallDistance = 1f;
    
    public void WaterFall()
    {
        transform.DOKill();

        transform.DOLocalMoveY(transform.localPosition.y - fallDistance, fallDuration)
            .SetEase(Ease.InQuad); 
    }
}