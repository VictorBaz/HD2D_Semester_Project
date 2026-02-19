using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public event System.Action<Collider> EnteredTrigger;
    public event System.Action<Collider> ExitedTrigger;

    public void OnTriggerEnter(Collider other)
    {
        EnteredTrigger?.Invoke(other);
    }

    public void OnTriggerExit(Collider other)
    {
        ExitedTrigger?.Invoke(other);
    }
}
