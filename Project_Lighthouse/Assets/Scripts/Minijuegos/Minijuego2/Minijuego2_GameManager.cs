using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minijuego2_GameManager : MonoBehaviour
{
    public GameObject Comida_Cortada;
    private Camera cam;
    public bool imGrabing = false;
    private bool imCutting;
    private GameObject grabObject;
    private Selectable_MG2 grabObjectData;
    private float ScreenWidth, ScreenHeight;
    [Range(50,150)]public float camaraUmbral;
    public float limitRotationCamera;
    public float speedRotation;
    private float cameraRotation;
    private Vector2 moveDirection;
    private float moveSpeed = 0.4f;
    private bool _finishRecipe;
    public bool finishRecipe
    {
        get
        {
            return _finishRecipe;
        }
        set
        {
            _finishRecipe = value;
            if(_finishRecipe == true)
            {
                //Terminada la receta
                Debug.Log("Terminado");
            }
        }
    }
    
    void Start()
    {
        cam = Camera.main;
        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;
    }


    void Update()
    {
        CameraMovement();
    }

    public void CameraMovement()
    {
        if (Input.mousePosition.x > ScreenWidth - camaraUmbral)
        {
            if (cameraRotation < limitRotationCamera)
            {
                cam.transform.eulerAngles += new Vector3(0, 1, 0) * Time.deltaTime * speedRotation;
                cameraRotation += Time.deltaTime * speedRotation;
            }
        }
        if (Input.mousePosition.x < camaraUmbral)
        {
            if (cameraRotation > -limitRotationCamera)
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

                /*if (grabObject.tag == "Cuchillo")
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    grabObject.GetComponent<Knife>().isGrabbed = true;
                }

                if(grabObject.tag == "Comida")
                {
                    Cursor.lockState= CursorLockMode.Locked;
                    Cursor.visible = false;
                    grabObject.GetComponent<Comida>().isGrabbed = true;
                }
                */

                if(grabObject.GetComponent<Selectable_MG2>() != null)
                {
                    grabObjectData = grabObject.GetComponent<Selectable_MG2>();
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    grabObjectData.isGrabbed = true;
                    imGrabing = true;
                }
                if (grabObject.tag == "Olla")
                {
                    if (grabObject.GetComponent<Olla>().isFilledWithFood == true)
                    {
                        grabObject.GetComponent<Olla>().takeOffFood = true;
                        grabObject = grabObject.GetComponent<Olla>().foodCooked;
                        grabObjectData = grabObject.GetComponent<Selectable_MG2>();
                    }
                }
            }
            
        }
        else if(imGrabing && context.performed)
        {

            switch (grabObject.gameObject.tag)
            {
                case "Cuchillo":
                    grabObjectData.isGrabbed = false;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    grabObject.GetComponent<Selectable_MG2>().moveDirection = Vector2.zero;
                    grabObject.transform.DOMove(grabObject.GetComponent<Selectable_MG2>().origPosition, 0.75f);
                    grabObject = null;
                    grabObjectData = null;
                    break;
                case "Comida":
                    grabObjectData.isGrabbed = false;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    grabObject.GetComponent<Selectable_MG2>().moveDirection = Vector2.zero;
                    grabObject = null;
                    grabObjectData = null;
                    break;
                case "Comida_Hecha":
                    grabObjectData.isGrabbed = false;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    grabObject.GetComponent<Selectable_MG2>().moveDirection = Vector2.zero;
                    grabObject = null;
                    grabObjectData = null;
                    break;
            }
            
            imGrabing = false;
        }
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (imGrabing)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection = moveDirection * Time.deltaTime * moveSpeed;
            grabObjectData.moveDirection = moveDirection;
        }
    }

    public void PutFoodInPot(Comida comida)
    {
        comida.GetComponent<Selectable_MG2>().isGrabbed = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        comida.GetComponent<Selectable_MG2>().moveDirection = Vector2.zero;
        grabObject = null;
    }

    public void OnCut(InputAction.CallbackContext context)
    {
        if (imGrabing)
        {
            switch (grabObject.gameObject.tag)
            {
                case "Untagged":
                    Debug.LogWarning("The grabbed object hasn't any tag");
                    break;
                case "Cuchillo":
                    Selectable_MG2 knifeObjData = grabObject.GetComponent<Selectable_MG2>();
                    Knife knife = grabObject.GetComponent<Knife>();
                    if (context.performed && knifeObjData.isGrabbed)
                    {
                        imCutting = true;
                    }
                    else if (context.canceled && knifeObjData.isGrabbed)
                    {
                        imCutting = false;
                    }
                    if (context.performed && knife.thereIsFood && knife.feedbackSupervisor)
                    {
                        Comida comida_Cortada = grabObject.GetComponent<Knife>().Comida.GetComponent<Comida>();
                        if (comida_Cortada.canBeCutted)
                        {
                            //Destroy(Comida.transform.Find("Forma").gameObject);
                            Instantiate(comida_Cortada.comida_Cortada, grabObject.GetComponent<Knife>().Comida.transform);
                            comida_Cortada.isCutted = true;
                            knife.thereIsFood = false;
                        }
                        else
                        {
                            // Feedback de que la comida hace falta pelarla o que aun no se puede cortar
                            comida_Cortada.gameObject.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => knife.feedbackSupervisor = false).OnComplete(() => knife.feedbackSupervisor = true);
                        }

                    }
                    break;
            } //REcuerda hacer lo mismo con todos los componentes que tengan un script con la funcion de input OnCut, para generalizarlos todos en el game manager y que sea mas versátil a la hora de trabajar
        }
    }
}
