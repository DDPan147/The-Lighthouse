using System;
using UnityEngine;
using UnityEngine.InputSystem; 


public class PlayerGrabObjects : MonoBehaviour
{
    [SerializeField] private Player_Movement_Minigame_7 playerMovement;
    public GameObject objectToGrab;
    public Collider detectionCollider;
    [SerializeField] private bool canGrab = false;
    [SerializeField] private Vector3 _objectPositionOffset;
    
    [Header("Papelera")]
    [SerializeField] private GameObject keyPromptUI; // Referencia al popup UI
    private bool isNearTrashbin = false;
    
    private void Start()
    {
        playerMovement = GetComponent<Player_Movement_Minigame_7>();
        OnTriggerEnter(detectionCollider);
        if (keyPromptUI != null)
            keyPromptUI.SetActive(false);
    }
    //Rotamos el collider en caso de que el jugador este moviendose por el mapa
    private void Update()
    {
        if (playerMovement.inputDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(playerMovement.inputDirection.x, playerMovement.inputDirection.y) * Mathf.Rad2Deg;
            detectionCollider.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    //Suscribimos el evento para poder agarrar el objeto
    public void OnGrab(InputAction.CallbackContext context)
    {
        GrabObject();
    }
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed && objectToGrab != null && isNearTrashbin)
        {
            ThrowObject();
        }
    }
    //OntriggerEnter para poder comprobar si se encuentra el objeto
    private void OnTriggerEnter(Collider other)
    {
        CheckTag(other);
    }
    
    //Y ontriggerexit para poner las variables en default
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Objeto"))
        {
            canGrab = false;
            objectToGrab = null;
            Debug.Log("Saliste del objeto");
        }
        else if (other.CompareTag("Trashbin"))
        {
            isNearTrashbin = false;
            // Ocultar el popup
             if (keyPromptUI != null)
                 keyPromptUI.SetActive(false);
            Debug.Log("Alejado de la papelera");
        }
    }
    //Agarramos el objeto en caso de que el collider encuentre un objeto
    private void GrabObject()
    {
        if (canGrab && objectToGrab != null)
        {
            Debug.Log("Objeto capturado");
            objectToGrab.transform.parent = transform;
            objectToGrab.transform.localPosition = _objectPositionOffset;
            Collider collider = objectToGrab.GetComponent<Collider>();
            collider.enabled = false;
            canGrab = false;
        }
        else
        {
            Debug.Log("No hay objeto para capturar");
        }
    }
    private void CheckTag(Collider other)
    {
        if (other.CompareTag("Objeto"))
        {
            objectToGrab = other.gameObject;
            canGrab = true;
            Debug.Log("Objeto encontrado");
        }
        else if (other.CompareTag("Trashbin") && objectToGrab != null)
        {
            isNearTrashbin = true;
            // Mostrar el popup
            if (keyPromptUI != null)
                keyPromptUI.SetActive(true);
            Debug.Log("Cerca de la papelera");
        }
    }
    private void ThrowObject()
    {
        if (objectToGrab != null)
        {
            // Destruir el objeto
            Destroy(objectToGrab);
            objectToGrab = null;
            
            // Ocultar el popup
            if (keyPromptUI != null)
                keyPromptUI.SetActive(false);
            
            Debug.Log("Objeto tirado a la papelera");
        }
    }
    
}
