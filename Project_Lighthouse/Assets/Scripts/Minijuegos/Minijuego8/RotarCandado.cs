using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class RotarCandado : MonoBehaviour
{
    private Camera cam;
    public GameObject canica, candado;
    private float rotation;
    public bool canRotate = true;
    void Start()
    {
        cam = Camera.main;
    }


    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Tuberia") && canRotate)
                { 
                    canRotate = false;
                    rotation += -90;
                    transform.DORotate(new Vector3(candado.transform.localEulerAngles.x,candado.transform.localEulerAngles.y, rotation), 0.5f, RotateMode.Fast).OnComplete(() => canica.GetComponent<Pelota>().MoveMarble());
                }
                if (hit.collider.gameObject.CompareTag("Tuberia_Entrada") && canRotate)
                { 
                    canRotate = false;
                    rotation += 90;
                    transform.DORotate(new Vector3(candado.transform.localEulerAngles.x,candado.transform.localEulerAngles.y, rotation), 0.5f, RotateMode.Fast).OnComplete(() => canica.GetComponent<Pelota>().MoveMarble());
                }
            }
        }
        
    }

    public void Win()
    {
        Debug.Log("Has Ganado");
        candado.transform.DORotate(new Vector3(0, 90, 0), 1.5f, RotateMode.Fast);
    }

    public void CanRotate()
    {
        canRotate = true;
    }

    public void CantRotate()
    {
        canRotate = false;
    }
}
