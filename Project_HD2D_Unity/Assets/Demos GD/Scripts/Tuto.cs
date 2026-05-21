using System;
using UnityEngine;

public class Tuto : MonoBehaviour
{
    [SerializeField] private GameObject popUp;
    private Transform _canvasTransform;

    private void Awake()
    {
        _canvasTransform = GameObject.FindGameObjectWithTag("Canvas").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(popUp, _canvasTransform);
        gameObject.SetActive(false);
    }
}
