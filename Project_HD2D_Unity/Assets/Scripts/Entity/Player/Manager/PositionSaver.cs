using System;
using UnityEngine;

public class PositionSaver
{
    private readonly Vector3[] positions;
    private readonly int size;
    private int head = 0;

    public PositionSaver(int size)
    {
        this.size = size;
        positions = new Vector3[size];
    }

    public void Save(Vector3 position, bool canSave)
    {
        if (!canSave) return;

        head = (head - 1 + size) % size;
        positions[head] = position;
    }

    public Vector3 Get(int index) => positions[(head + index) % size];
    public Vector3 Latest => positions[head];
}