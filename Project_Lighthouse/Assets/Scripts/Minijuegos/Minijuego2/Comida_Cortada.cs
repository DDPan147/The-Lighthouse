using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Comida_Cortada : MonoBehaviour
{
    public bool isGrabbed;
    [HideInInspector] public bool isCutted;
    [HideInInspector] public Vector2 moveDirection;
    private float moveSpeed = 0.4f;
    private bool thereIsBread;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PanRallado"))
        {
            thereIsBread = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PanRallado"))
        {
            thereIsBread = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isGrabbed)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection = moveDirection * Time.deltaTime * moveSpeed;
        }
    }

    public void OnCut(InputAction.CallbackContext context)
    {
        if (context.performed && thereIsBread && isGrabbed)
        {
            GetComponentInChildren<Renderer>().material.color = Color.red;
        }
    }
}
