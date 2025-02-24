using DG.Tweening;
using UnityEngine;

public class Barco : MonoBehaviour
{
    public Vector3 direction;
    private bool inLight;
    private float speed = 0.5f;
    private Rigidbody rb;
    public enum Personalidad
    {
        Depresion,
        Personalidad2,
        Personalidad3
    }
    public Personalidad personalidad;
    private GameObject luz;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        luz = GameObject.FindGameObjectWithTag("Luz");
        //transform.DOLookAt(luz.transform.position, 0.5f, axisConstraint: AxisConstraint.X, Vector3.up);
    }


    void Update()
    {
        if (inLight)
        {
            
        }
        if (!inLight)
        {
            transform.LookAt(luz.transform, Vector3.up);
            MoveToLight();
        }
    }

    public void MoveToLight()
    {
        direction = luz.transform.position - transform.position;
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == luz.tag)
        {
            inLight = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == luz.tag)
        {
            transform.DOLookAt(luz.transform.position, 0.5f,AxisConstraint.None ,Vector3.up).OnComplete(() =>
            {
                inLight = false;
            });
        }
    }
}
