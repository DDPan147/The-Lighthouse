using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class CameraCollider : MonoBehaviour
{
    /*Camera Switching*/
    public CinemachineCamera cam;
    private Player player;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("I am " + gameObject.name + " and I have priority");
            cam.Priority += 3;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("I am " + gameObject.name + " and I no longer have priority");
            cam.Priority -= 3;
        }
    }
}
