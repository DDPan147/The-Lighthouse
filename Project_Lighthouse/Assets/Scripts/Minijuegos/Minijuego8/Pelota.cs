using UnityEngine;

public class Pelota : MonoBehaviour
{
    private Rigidbody rb;
    //public GameObject canica;

    public Vector3 whatIsDown;  
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        whatIsDown = -transform.up;
        rb.linearVelocity = whatIsDown;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    public void MoveMarble()
    {
        rb.linearVelocity = whatIsDown;
    }
}
