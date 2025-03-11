using UnityEngine;

public class LightMovement : MonoBehaviour
{
    private Camera cam;
    public LayerMask lightFloor;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        
    }


    void Update()
    {
        MouseRaycast();
    }

    void MouseRaycast()
    {
        Ray ray = cam.ScreenPointToRay( Input.mousePosition );
        RaycastHit hit;

        if(Physics.Raycast( ray, out hit, Mathf.Infinity, lightFloor))
        {
            transform.position = hit.point;
        }
    }
}
