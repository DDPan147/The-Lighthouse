using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class RotarCandado : MonoBehaviour
{
    private Camera cam;
    public GameObject canica, candado;
    private float rotation;
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
                if (hit.collider.gameObject.CompareTag("Tuberia"))
                { 
                    rotation += -90;
                    candado.transform.DORotate(new Vector3(candado.transform.localEulerAngles.x,candado.transform.localEulerAngles.y, rotation), 0.5f, RotateMode.Fast).OnComplete(() => canica.GetComponent<Pelota>().MoveMarble());
                }
            }
        }
        
    }
}
