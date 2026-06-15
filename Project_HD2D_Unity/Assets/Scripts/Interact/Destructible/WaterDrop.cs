using UnityEngine;
using DG.Tweening; 

public class WaterDrop : MonoBehaviour
{
    [SerializeField] private float fallDuration = 1f;
    [SerializeField] private Vector3 fallVector;
    
    public void WaterFall()
    {
        transform.DOKill();

        transform.DOLocalMove(fallVector, fallDuration)
            .SetEase(Ease.InQuad); 
    }
}