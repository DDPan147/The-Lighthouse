using UnityEngine;
using UnityEngine.InputSystem;

public class Knife : MonoBehaviour
{
    public bool isCutting;
    private Vector2 mousePos;
    private Vector3 point;
    private Camera cam;
    private Vector3 rayDirection;
    private GameObject tablaCortar;

    private void Start()
    {
        cam = Camera.main;
        tablaCortar = GameObject.Find("TabladeCortar");
        rayDirection = tablaCortar.transform.position - transform.position;
    }

    private void Update()
    {
        mousePos.x = Input.mousePosition.x;
        mousePos.y = cam.pixelHeight - Input.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));


        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0, -1, 0.5f), out hit);
        Debug.DrawLine(point, hit.point);

        transform.position = hit.point;
    }
    public void OnCut(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            isCutting = true;
        }
        else if(context.canceled)
        {
            isCutting = false;
        }
    }
}
