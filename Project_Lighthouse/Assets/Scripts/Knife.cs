using UnityEngine;
using UnityEngine.InputSystem;

public class Knife : MonoBehaviour
{
    public bool isCutting;
    private Vector2 moveDirection;
    private float moveSpeed = 0.4f;
    private Vector3 originalPosition;
    private bool hasStarted;
    public Transform minLimitX, minLimitZ, maxLimitX, maxLimitZ;

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
