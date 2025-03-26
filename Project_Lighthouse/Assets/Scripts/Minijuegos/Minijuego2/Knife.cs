using UnityEngine;
using UnityEngine.InputSystem;

public class Knife : MonoBehaviour
{
    public bool isCutting;
    public GameObject Comida_Cortada;
    [HideInInspector] public Vector2 moveDirection;
    private float moveSpeed = 0.4f;
    [HideInInspector] public Vector3 origPosition;
    private bool thereIsFood;
    public Transform minLimitX, minLimitZ, maxLimitX, maxLimitZ;
    private GameObject Comida;
    public bool isGrabbed;


    private void Start()
    {
        origPosition = transform.position;
    }

    private void Update()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
        
        
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
        if (context.performed && isGrabbed)
        {
            isCutting = true;
        }
        else if(context.canceled && isGrabbed)
        {
            isCutting = false;
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

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isGrabbed)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection = moveDirection * Time.deltaTime * moveSpeed;
            LimitarMovimiento();
        }
    }


}
