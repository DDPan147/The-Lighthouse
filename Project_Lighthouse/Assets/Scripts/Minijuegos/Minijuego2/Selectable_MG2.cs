using UnityEngine;

public class Selectable_MG2 : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed;
    [HideInInspector] public Vector2 moveDirection;
    private float moveSpeed = 0.4f;
    void Start()
    {
        
    }


    void Update()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
    }
}
