using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomActivationScript : MonoBehaviour
{
    #region variables
    
    public List<GameObject> RoomList = new List<GameObject>();

    #endregion

    #region UnityLifeCycle

    private void Start()
    {
        if (RoomList.Count > 0)
        {
            ActivateRoom(0);
        }
        else
        {
            Debug.LogWarning("Room Activation/Deactivation is not used");
        }
    }

    #endregion

    #region Fonction

    public void ActivateRoom(int room)
    {
        if (room < 0 || room >= RoomList.Count)
        {
            Debug.LogWarning("Invalid room index: " + room);
            return;
        }

        for (int i = 0; i < RoomList.Count; i++)
        {
            bool shouldBeActive = (i == room || i == room - 1 || i == room + 1);
            RoomList[i].SetActive(shouldBeActive);
        }
    }

    #endregion
}