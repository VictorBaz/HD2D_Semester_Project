using UnityEngine;

public class PreviewEjectionPlayer
{
    public GameObject  PreviewElement           { get; private set; }
    public LineRenderer TrajectoryLineRenderer  { get; private set; }
    public Transform    TrajectoryStartTransform { get; private set; }

    private const int   TrajectoryResolution = 300;
    private const float TimeStep            = 0.05f;
    private Vector3 normalPreviewElement;

    public PreviewEjectionPlayer(
        GameObject previewElement,
        LineRenderer trajectoryLineRenderer,
        Transform trajectoryStartTransform)
    {
        PreviewElement            = previewElement;
        TrajectoryLineRenderer    = trajectoryLineRenderer;
        TrajectoryStartTransform  = trajectoryStartTransform;

        TrajectoryLineRenderer.positionCount = TrajectoryResolution;
        previewElement.transform.SetParent(null);
        TogglePreview(false);
    }

    public void TogglePreview(bool on)
    {
        PreviewElement.SetActive(on);
        TrajectoryLineRenderer.enabled = on;
    }

    public void UpdatePreviewElementPosition(PlayerStateContext psc)
    {
        PreviewElement.transform.position = psc.TrajectoryPoint;
        PreviewElement.transform.up = normalPreviewElement;
    }

    public Vector3? UpdateTrajectory(Vector3 force)
    {
        TrajectoryLineRenderer.positionCount = TrajectoryResolution;

        Vector3 origin   = TrajectoryStartTransform.position;
        Vector3 velocity = force;

        for (int i = 0; i < TrajectoryResolution; i++)
        {
            float   t     = i * TimeStep;
            Vector3 point = origin + velocity * t + Physics.gravity * (0.5f * t * t);

            if (i > 0)
            {
                Vector3 prev = TrajectoryLineRenderer.GetPosition(i - 1);
                Vector3 dir  = point - prev;
                float   dist = dir.magnitude;

                if (Physics.Raycast(prev, dir.normalized, out RaycastHit hit, dist))
                {
                    TrajectoryLineRenderer.positionCount = i;
                    normalPreviewElement = hit.normal;
                    return hit.point;
                }
            }

            TrajectoryLineRenderer.SetPosition(i, point);
        }

        return null;
    }
}