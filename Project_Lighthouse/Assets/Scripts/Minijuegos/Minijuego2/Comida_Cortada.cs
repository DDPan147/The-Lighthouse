using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Comida_Cortada : MonoBehaviour
{
    public bool isGrabbed;
    [HideInInspector] public Vector2 moveDirection;
    private float moveSpeed = 0.4f;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
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
