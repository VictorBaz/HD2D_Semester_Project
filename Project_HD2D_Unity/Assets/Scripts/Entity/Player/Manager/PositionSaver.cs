using System.Collections.Generic;
using UnityEngine;

public class PositionSaver
{
    private List<Vector3> positions;
    private readonly int maxSize;

    public PositionSaver(int maxSize)
    {
        this.maxSize = maxSize;
        positions = new List<Vector3>(maxSize);
    }

    public void Save(Vector3 position, bool canSave)
    {
        if (!canSave) return;

        positions.Insert(0, position);
        if (positions.Count > maxSize)
        {
            positions.RemoveAt(positions.Count - 1);
        }
    }

    public Vector3 Get(int index) 
    {
        if (positions.Count == 0) return Vector3.zero; 
        return positions[Mathf.Clamp(index, 0, positions.Count - 1)];
    }

    public Vector3 Latest => positions.Count > 0 ? positions[0] : Vector3.zero;
    
   
    public Vector3 GetRespawnPosition() => Get(positions.Count / 2);

    
    public void Clear() => positions.Clear();
    
    public bool HasData => positions.Count > 0;
    public int Count => positions.Count;
}