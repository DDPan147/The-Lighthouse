using UnityEngine;
using UnityEngine.InputSystem;

public class Knife : MonoBehaviour
{
    public bool isCutting;
    public GameObject Comida_Cortada;
    private Vector2 moveDirection;
    private float moveSpeed = 0.4f;
    private Vector3 originalPosition;
    private bool hasStarted;
    private bool thereIsFood;
    public Transform minLimitX, minLimitZ, maxLimitX, maxLimitZ;
    private GameObject Comida;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    private void Update()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);

        if(transform.position.x < minLimitX.position.x)
        {
            transform.position = new Vector3(minLimitX.position.x, transform.position.y, transform.position.z);
        }
        else if(transform.position.x > maxLimitX.position.x)
        {
            transform.position = new Vector3(maxLimitX.position.x, transform.position.y, transform.position.z);
        }
        else if(transform.position.z < minLimitZ.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, minLimitZ.position.z);
        }
        else if(transform.position.z > maxLimitZ.position.z)
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
        if (isCutting)
        {
            Instantiate(Comida_Cortada, Comida.transform.position, Quaternion.Euler(0, 0, 90));
            Destroy(Comida);
            thereIsFood = false;
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
        hasStarted = true;
        if (context.started)
        {
            isCutting = true;
        }
        else if(context.canceled)
        {
            isCutting = false;
        }
        if (context.performed && thereIsFood)
        {
            Instantiate(Comida_Cortada, Comida.transform.position, Quaternion.Euler(0,0,90));
            Destroy(Comida);
            thereIsFood = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (hasStarted)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection = moveDirection * Time.deltaTime * moveSpeed;
        }
    }

}
