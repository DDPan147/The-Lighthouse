using UnityEngine;
using UnityEngine.InputSystem;

public class Luz_Movement : MonoBehaviour
{
    private Vector2 moveDirection;
    private float moveSpeed = 0.25f;
    private bool selected;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(selected)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection *= Time.deltaTime * moveSpeed;
        }
    }

    public void OnCut(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(selected)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                selected = false;
            }
            if (!selected)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                selected = true;
            }
        }
    }
}
