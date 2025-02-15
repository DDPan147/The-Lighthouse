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
    
    private void Start()
    {
        playerMovement = GetComponent<Player_Movement_Minigame_7>();
        OnTriggerEnter(detectionCollider);
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
    //OntriggerEnter para poder comprobar si se encuentra el objeto
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objeto"))
        {
            objectToGrab = other.gameObject;
            canGrab = true;
            Debug.Log("Objeto encontrado");
        }
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
    }
    //Agarramos el objeto en caso de que el collider encuentre un objeto
    private void GrabObject()
    {
        if (canGrab && objectToGrab != null)
        {
            Debug.Log("Objeto capturado");
            objectToGrab.transform.parent = transform;
            objectToGrab.transform.localPosition = _objectPositionOffset;
            canGrab = false;
        }
        else
        {
            Debug.Log("No hay objeto para capturar");
        }
    }

    
}
