using System;
using UnityEngine;

public class PlayerGrabObjects : MonoBehaviour
{
    public GameObject objectToGrab;
    public Collider detectionCollider;

    private void Start()
    {
        OnTriggerEnter(detectionCollider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objeto"))
        {
            Debug.Log("Objeto encontrado");
        }
    }
}
