using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Comida_Hecha : MonoBehaviour
{
    public bool isGrabbed;
    private Vector2 moveDirection;
    private float moveSpeed = 0.4f;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isGrabbed)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection = moveDirection * Time.deltaTime * moveSpeed;
        }
    }
}
