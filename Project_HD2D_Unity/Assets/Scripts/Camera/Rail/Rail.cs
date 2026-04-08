using UnityEngine;

public class Rail : MonoBehaviour 
{
    private Vector3[] nodes;
    private int nodeCount;

    public int NodeCount => nodeCount;
    public Vector3[] Nodes => nodes;
    
    private void Start()
    {
        UpdateNodes();
    }

    
    public void UpdateNodes()
    {
        nodeCount = transform.childCount;
        if (nodes == null || nodes.Length != nodeCount)
        {
            nodes = new Vector3[nodeCount];
        }

        for (int i = 0; i < nodeCount; i++)
        {
            nodes[i] = transform.GetChild(i).position;
        }
    }

    #region Rail Logic
    public Vector3 ProjectPositionOnRail(Vector3 pos)
    {
        if (nodes == null || nodes.Length < 2) UpdateNodes();

        int closestNodeIndex = GetClosestNode(pos);

        if (closestNodeIndex == 0)
        {
            return ProjectOnSegment(nodes[0], nodes[1], pos);
        }

        if (closestNodeIndex == nodeCount - 1)
        {
            return ProjectOnSegment(nodes[nodeCount - 1], nodes[nodeCount - 2], pos);
        }

        Vector3 leftSeg = ProjectOnSegment(nodes[closestNodeIndex - 1], nodes[closestNodeIndex], pos);
        Vector3 rightSeg = ProjectOnSegment(nodes[closestNodeIndex + 1], nodes[closestNodeIndex], pos);
            
        return (pos - leftSeg).sqrMagnitude <= (pos - rightSeg).sqrMagnitude ? leftSeg : rightSeg;
    }

    private int GetClosestNode(Vector3 pos)
    {
        int closestNodeIndex = -1;
        float shortestDistance = float.MaxValue;

        for (int i = 0; i < nodeCount; i++)
        {
            float sqrDistance = (nodes[i] - pos).sqrMagnitude;
            if (sqrDistance < shortestDistance)
            {
                shortestDistance = sqrDistance;
                closestNodeIndex = i;
            }
        }
        return closestNodeIndex;
    }

    public Vector3 ProjectOnSegment(Vector3 v1, Vector3 v2, Vector3 pos)
    {
        Vector3 v1ToPos = pos - v1;
        Vector3 v1ToV2 = v2 - v1;
        Vector3 segDirection = v1ToV2.normalized;

        float distanceFromV1 = Vector3.Dot(segDirection, v1ToPos);

        if (distanceFromV1 < 0.0f) return v1;
        if (distanceFromV1 * distanceFromV1 > v1ToV2.sqrMagnitude) return v2;

        return v1 + segDirection * distanceFromV1;
    }
    #endregion

    #region Editor Gizmos
    private void OnDrawGizmos()
    {
        UpdateNodes();

        if (nodes == null || nodes.Length < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            Gizmos.DrawLine(nodes[i], nodes[i + 1]);
            Gizmos.DrawSphere(nodes[i], 0.1f);
        }
        Gizmos.DrawSphere(nodes[nodeCount - 1], 0.1f);
    }
    #endregion
}