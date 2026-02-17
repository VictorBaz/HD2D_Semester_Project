using UnityEngine;

public class RailMover : MonoBehaviour {

    public Rail rail;
    public Transform lookAt;
    public bool smothMove = true;
    public float moveSpeed;

    private Transform thisTransform;
    private Vector3 lastPosition;
    void Start () {

        thisTransform = transform;
    }
 
 
    void Update ()
    {
        if (smothMove)
        {
            lastPosition = Vector3.Lerp(lastPosition, rail.ProjectPositionOnRail(lookAt.position), Time.deltaTime * moveSpeed);
            thisTransform.position = lastPosition;
        }
        else
        {
            thisTransform.position = rail.ProjectPositionOnRail(lookAt.position);
        }
        thisTransform.LookAt(lookAt.position);
    }
}