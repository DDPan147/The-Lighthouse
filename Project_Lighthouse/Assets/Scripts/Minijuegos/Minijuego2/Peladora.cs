using UnityEngine;
using UnityEngine.InputSystem;

public class Peladora : MonoBehaviour
{
    public bool isPelando;
    public GameObject Comida_Pelada;
    [HideInInspector] public Vector3 origPosition;
    private bool thereIsFood;
    public Transform minLimitX, minLimitZ, maxLimitX, maxLimitZ;
    private GameObject Comida;
    //public bool isGrabbed;
    private Selectable_MG2 objData;


    private void Start()
    {
        origPosition = transform.position;
        objData = GetComponent<Selectable_MG2>();
    }

    private void Update()
    {
        //transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
        LimitarMovimiento();
    }
    void LimitarMovimiento()
    {
        if (transform.position.x < minLimitX.position.x)
        {
            transform.position = new Vector3(minLimitX.position.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > maxLimitX.position.x)
        {
            transform.position = new Vector3(maxLimitX.position.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.z < minLimitZ.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, minLimitZ.position.z);
        }
        else if (transform.position.z > maxLimitZ.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxLimitZ.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Comida"))
        {
            thereIsFood = true;
            Comida = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Comida"))
        {
            thereIsFood = false;
        }
    }


    public void OnCut(InputAction.CallbackContext context)
    {
        if (context.performed && objData.isGrabbed)
        {
            isPelando = true;
        }
        else if(context.canceled && objData.isGrabbed)
        {
            isPelando = false;
        }
        if (context.performed && thereIsFood)
        {
            Comida comida_Cortada = Comida.GetComponent<Comida>();
            Destroy(Comida.transform.Find("Forma").gameObject);
            Instantiate(comida_Cortada.comida_Cortada, Comida.transform);
            //Comida.transform.Find("ComidaCortada_Minijuego2").gameObject.SetActive(true);
            //Comida.transform.Find("Comida_Minijuego2").gameObject.SetActive(false);
            comida_Cortada.isCutted = true;
            thereIsFood = false;
        }
    }

    /*public void OnMove(InputAction.CallbackContext context)
    {
        if (isGrabbed)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection = moveDirection * Time.deltaTime * moveSpeed;
            LimitarMovimiento();
        }
    }*/


}
