using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minijuego2_GameManager : MonoBehaviour
{
    [Header("Scene Objects")]

    public GameObject olla;
    public GameObject sarten;
    public GameObject platoFishChips, plato2;
    public GameObject recetaFishChips, receta2;
    public GameObject ingredientesReceta;
    [Space(20)]
    public GameObject Comida_Cortada;
    public GameObject huesosPescado;
    //public GameObject PlatoPrefab;
    public TMP_Text nombrePlato;
    private Camera cam;
    public bool imGrabing = false;
    private bool canGrab = true;
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
    #region Varibles que indican que el minijuego a terminado
    private bool _finishRecipe1;
    public bool finishRecipe1
    {
        get
        {
            return _finishRecipe1;
        }
        set
        {
            _finishRecipe1 = value;
            if(_finishRecipe1 == true)
            {
                //Terminada la primera receta
                NextReciepe();
            }
        }
    }
    private bool _finishRecipe2;
    public bool finishRecipe2
    {
        get
        {
            return _finishRecipe2;
        }
        set
        {
            _finishRecipe2 = value;
            if(_finishRecipe2 == true)
            {
                //Minijuego Terminado
                Debug.Log("Terminado");
            }
        }
    }
    #endregion
    void Start()
    {
        cam = Camera.main;
        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;
        nombrePlato.text = "Fish&Chips" + ":";
        plato2.GetComponent<BoxCollider>().enabled = false;
        platoFishChips.GetComponent<BoxCollider>().enabled = true;
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
        ingredientesReceta.SetActive(true);
        recetaFishChips.SetActive(false);
        receta2.SetActive(true);
        MoveDishesSequence();
        platoFishChips.GetComponent<BoxCollider>().enabled = false;
        plato2.GetComponent<BoxCollider>().enabled = true;
    }

    void MoveDishesSequence()
    {
        Vector3 plato1Position = platoFishChips.transform.position;
        Vector3 ollaPosition = olla.transform.position;
        Vector3 sartenPosition = sarten.transform.position;
        Sequence MoveObjects = DOTween.Sequence();
        MoveObjects.Append(platoFishChips.transform.DOMoveZ(-1.5f, 1.75f, false));  //Mueve el plato ya preparado hacia adelante
        MoveObjects.Append(plato2.transform.DOMove(plato1Position, 1f, false));     //Mueve el plato vacio cerca de la zona de trabajo/cocina
        MoveObjects.Append(olla.transform.DOMoveY(0.5f, 0.5f, false));              //Levanta la olla
        MoveObjects.Append(olla.transform.DOMoveX(sartenPosition.x, 1.25f, false)); //Mueve la olla hacia donde estaba la sarten
        MoveObjects.Append(sarten.transform.DOMoveX(ollaPosition.x, 1.25f, false)); //Mueve la sarten hacia donde estaba la olla
        MoveObjects.Append(olla.transform.DOMoveY(sartenPosition.y, 0.5f, false));  //Deja la olla donde estaba anteriormente la sarten
        MoveObjects.OnPlay(() => canGrab = false);
        MoveObjects.OnComplete(() => canGrab = true);
    }
    public void PutFoodInPot(Comida comida)
    {
        comida.GetComponent<Selectable_MG2>().isGrabbed = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        comida.GetComponent<Selectable_MG2>().moveDirection = Vector2.zero;
        grabObject = null;
    }
    #region InputEvents
    public void OnGrab(InputAction.CallbackContext context)
    {
        if (!imGrabing && context.performed && canGrab)
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
                        grabObject.GetComponent<Olla>().isFilledWithFood = false;
                        //grabObject = grabObject.GetComponent<Olla>().foodCooked;
                        //grabObjectData = grabObject.GetComponent<Selectable_MG2>();
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
                            if(comida_Cortada.tipoComida == Comida.TipoComida.Pescado)
                            {
                                Instantiate(huesosPescado, new Vector3(comida_Cortada.transform.position.x, comida_Cortada.transform.position.y, comida_Cortada.transform.position.z + 0.25f), Quaternion.identity, ingredientesReceta.transform.parent.transform);
                            }
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
                    if (context.performed && comida.thereIsBread && comidaObjData.isGrabbed && comida.isCutted && comida.canBeRebozado)
                    {
                        comida.isRebozado = true;
                    }
                    else if (!comida.canBeRebozado && comida.thereIsBread)
                    {
                        //Feedback visual de que no se puede rebozar
                        Debug.Log("No se puede rebozar");
                        comida.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => comida.feedbackSupervisor = false).OnComplete(() => comida.feedbackSupervisor = true);
                    }
                    else if (context.performed && !comida.isCutted && comida.canBeCutted && comidaObjData.isGrabbed && comida.thereIsBread)
                    {
                        //Feedback visual de que falta cortarlo
                        Debug.Log("Falta cortar");
                        comida.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => comida.feedbackSupervisor = false).OnComplete(() => comida.feedbackSupervisor = true);
                    }
                    else if(context.performed && !comida.isPelado && comida.canBePelado && comidaObjData.isGrabbed && comida.thereIsBread)
                    {
                        //Feedback visual de que falta pelarlo
                        Debug.Log("Falta pelar ");
                        comida.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => comida.feedbackSupervisor = false).OnComplete(() => comida.feedbackSupervisor = true);
                    }
                    break;
            } 
        }
    }
    #endregion
}
