using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minijuego2_GameManager : MonoBehaviour
{
    public GameObject Comida_Cortada;
    //public GameObject PlatoPrefab;
    public TMP_Text nombrePlato;
    private Camera cam;
    public bool imGrabing = false;
    private bool imCutting;
    private bool imPelando;
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
                //NextReciepe();
            }
        }
    }
    
    void Start()
    {
        cam = Camera.main;
        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;
        nombrePlato.text = "Fish&Chips" + ":";
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

    public void NextReciepe()
    {
        //Hacer que se inicie la segunda receta
        nombrePlato.text = "Caldo Pescado" + ":";
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

                if(grabObject.GetComponent<Selectable_MG2>() != null)
                {
                    grabObjectData = grabObject.GetComponent<Selectable_MG2>();
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    grabObjectData.isGrabbed = true;
                    imGrabing = true;
                    grabObject.transform.DOMoveY(0.3f, 0.25f, false);
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
                case "Peladora":
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
                    Debug.LogWarning("The grabbed object doesn't have any tag");
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
                        Comida comida_Cortada = knife.Comida.GetComponent<Comida>();
                        if (comida_Cortada.canBeCutted)
                        {
                            knife.Comida.transform.Find("Forma").gameObject.SetActive(false);
                            Instantiate(comida_Cortada.comida_Cortada, grabObject.GetComponent<Knife>().Comida.transform);
                            comida_Cortada.isCutted = true;
                            knife.thereIsFood = false;
                        }
                        else
                        {
                            // Feedback de que la comida hace falta pelarla o que no se puede cortar
                            Debug.Log("No se puede cortar");
                            comida_Cortada.gameObject.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => knife.feedbackSupervisor = false).OnComplete(() => knife.feedbackSupervisor = true);
                        }

                    }
                    break;
                case "Peladora":
                    Selectable_MG2 peladoraObjData = grabObject.GetComponent<Selectable_MG2>();
                    Peladora peladora = grabObject.GetComponent<Peladora>();
                    if (context.performed && peladoraObjData.isGrabbed)
                    {
                        imPelando = true;
                    }
                    else if (context.canceled && peladoraObjData.isGrabbed)
                    {
                        imPelando = false;
                    }
                    if (context.performed && peladora.thereIsFood && peladora.feedbackSupervisor)
                    {
                        Comida comida_Cortada = peladora.Comida.GetComponent<Comida>();
                        if (comida_Cortada.canBePelado)
                        {
                            peladora.Comida.transform.Find("Forma").gameObject.SetActive(false);
                            Instantiate(comida_Cortada.comida_Pelada, peladora.Comida.transform);
                            comida_Cortada.isPelado = true;
                            peladora.thereIsFood = false;
                        }
                        else
                        {
                            // Feedback de que la comida no se puede pelar
                            Debug.Log("No se puede pelar");
                            comida_Cortada.gameObject.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => peladora.feedbackSupervisor = false).OnComplete(() => peladora.feedbackSupervisor = true);
                        }

                    }
                    break;
                case "Comida":
                    Selectable_MG2 comidaObjData = grabObject.GetComponent<Selectable_MG2>();
                    Comida comida = grabObject.GetComponent<Comida>();
                    if (context.performed && comida.thereIsBread && comidaObjData.isGrabbed && comida.isCutted)
                    {
                        comida.isRebozado = true;
                    }
                    else if (context.performed && !comida.isCutted && comidaObjData.isGrabbed)
                    {
                        //Feedback visual de que falta cortarlode
                        Debug.Log("Falta cortar");
                        comida.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => comida.feedbackSupervisor = false).OnComplete(() => comida.feedbackSupervisor = true);
                    }
                    else if(context.performed && !comida.isPelado && comidaObjData.isGrabbed)
                    {
                        //Feedback visual de que falta pelarlo
                        Debug.Log("Falta pelar  ");
                        comida.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => comida.feedbackSupervisor = false).OnComplete(() => comida.feedbackSupervisor = true);
                    }
                    break;
            } 
        }
    }
}
