using System;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    #region Link

        public Camera cam;

    #endregion

    #region Variables

        [SerializeField] private bool freezeXZAxis = true;

    #endregion

    private void Update()
    {
        if (freezeXZAxis)
        {
            transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = cam.transform.rotation;
        }
    }
}
