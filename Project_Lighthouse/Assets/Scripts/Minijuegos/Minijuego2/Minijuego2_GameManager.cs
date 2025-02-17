using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minijuego2_GameManager : MonoBehaviour
{
    public GameObject Comida_Cortada;
    private Camera cam;
    public bool imGrabing = false;
    private GameObject grabObject;
    public float ScreenWidth, ScreenHeight;
    [Range(50,150)]public float camaraUmbral;
    public float limitRotationCamera;
    public float speedRotation;
    private float cameraRotation;
    void Start()
    {
        cam = Camera.main;
        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;
    }


    void Update()
    {
        if(Input.mousePosition.x > ScreenWidth - camaraUmbral)
        {
            if(cameraRotation < limitRotationCamera)
            {
                cam.transform.eulerAngles += new Vector3(0, 1, 0) * Time.deltaTime * speedRotation;
                cameraRotation += Time.deltaTime * speedRotation;
            }
        }
        if(Input.mousePosition.x < camaraUmbral)
        {
            if(cameraRotation > -limitRotationCamera)
            {
                cam.transform.eulerAngles -= new Vector3(0, 1, 0) * Time.deltaTime * speedRotation;
                cameraRotation -= Time.deltaTime * speedRotation;
            }
        }
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (!imGrabing && context.performed)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                grabObject = hit.collider.gameObject;

                if (grabObject.tag == "Cuchillo")
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    grabObject.GetComponent<Knife>().isGrabbed = true;
                }

                if(grabObject.tag == "Comida")
                {
                    Cursor.lockState= CursorLockMode.Locked;
                    Cursor.visible = false;
                    grabObject.GetComponent<Comida_Cortada>().isGrabbed = true;
                }
            }
            imGrabing = true;
        }
        else if(imGrabing && context.performed)
        {
            if(grabObject.gameObject.tag == "Cuchillo")
            {
                grabObject.GetComponent<Knife>().isGrabbed = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                grabObject.GetComponent<Knife>().moveDirection = Vector2.zero;
                grabObject.transform.DOMove(grabObject.GetComponent<Knife>().origPosition, 0.75f);
                grabObject = null;
            }
            else if(grabObject.gameObject.tag == "Comida")
            {
                grabObject.GetComponent<Comida_Cortada>().isGrabbed = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                grabObject.GetComponent<Comida_Cortada>().moveDirection = Vector2.zero;
                grabObject = null;
            }
            imGrabing = false;
        }
        
    }
}
