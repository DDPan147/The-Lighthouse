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

    

    public void MoveMarble()
    {
        if(SoundManager.instance != null)
        {
            SoundManager.instance.Play("BallRolling");
        }
        rb.linearVelocity = whatIsDown;
        rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
    }


}
