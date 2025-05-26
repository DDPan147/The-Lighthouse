using UnityEngine;

public class DownDetector : MonoBehaviour
{
    public GameObject canica;
    private Rigidbody rb;
    private Minigame8_GameManager gm;
    private void Awake()
    {
        rb = canica.GetComponent<Rigidbody>();
        gm = FindAnyObjectByType<Minigame8_GameManager>();
    }


    void Update()
    {
        transform.position = canica.transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstaculo"))
        {
            //rb.linearVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            gm.canRotate = true;
        }
    }
}
