using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; 


public class PlayerGrabObjects : MonoBehaviour
{
    [SerializeField] private Player_Movement_Minigame_7 playerMovement;
    public GameObject objectToGrab;
    public Collider detectionCollider;
    [SerializeField] private bool canGrab = false;
    [SerializeField] private Vector3 _objectPositionOffset;
    [SerializeField] private TypeObject _objectType;
    [SerializeField] private TMP_Text _playerText;
    private bool objectGrabbed = false;
    
    [Header("Papelera")]
    [SerializeField] private GameObject keyPromptUI;
    private bool isNearTrashbin = false;
    [SerializeField] private PlayEffectsTrashbin trashBinScript;
    
    [Header("Input Control")]
    private bool canSave = true; // Para evitar múltiples guardados
    private void Start()
    {
        playerMovement = GetComponent<Player_Movement_Minigame_7>();
        OnTriggerEnter(detectionCollider);
        if (keyPromptUI != null)
            keyPromptUI.SetActive(false);
        
        UpdatePromptText("");
    }
    
    private void UpdatePromptText(string message)
    {
        if (_playerText != null)
        {
            _playerText.text = message;
        }
    }
    //Rotamos el collider en caso de que el jugador este moviendose por el mapa
    private void Update()
    {
        if (playerMovement.inputDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(playerMovement.inputDirection.x, playerMovement.inputDirection.y) * Mathf.Rad2Deg;
            detectionCollider.transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        if (objectGrabbed && !isNearTrashbin)
        {
            UpdatePromptText("Press R to Save Object");
        }
    }
    #region Inputs

    //Suscribimos los inputs
    public void OnGrab(InputAction.CallbackContext context)
    {
        if (objectToGrab != null)
        {
            GrabObject();
        }
    }
    public void OnSaveObject(InputAction.CallbackContext context)
    {
        if (context.performed && canSave && objectToGrab != null && _objectType.isImportantObject)
        {
            SaveObject();
            canSave = false; // Prevenir múltiples guardados
            StartCoroutine(ResetSaveFlag());
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed && objectToGrab != null && isNearTrashbin)
        {
            ThrowObject();
            trashBinScript.PlayEffects();
        }
    }
    #endregion
    
    #region Triggers
    //OntriggerEnter para poder comprobar si se encuentra el objeto
    private void OnTriggerEnter(Collider other)
    {
        TypeObject typeOb = other.GetComponent<TypeObject>();
        if (typeOb != null)
        {
            _objectType = typeOb;
        }
        CheckTag(other);
    }
    
    //Y ontriggerexit para poner las variables en default
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Objeto"))
        {
            canGrab = false;
            objectToGrab = null;
            canSave = true;
            UpdatePromptText("");
        }
        else if (other.CompareTag("Trashbin"))
        {
            isNearTrashbin = false;
            // Ocultar el popup
             if (keyPromptUI != null)
                 keyPromptUI.SetActive(false);
             UpdatePromptText("");
        }
    }
    #endregion
    
    //Agarramos el objeto en caso de que el collider encuentre un objeto
    private void GrabObject()
    {
        if (canGrab && objectToGrab != null)
        {
            objectToGrab.transform.parent = transform;
            objectToGrab.transform.localPosition = _objectPositionOffset;
            Collider collider = objectToGrab.GetComponent<Collider>();
            collider.enabled = false;
            canGrab = false;
            objectGrabbed = true;
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
            UpdatePromptText("Press E to grab object");
            objectToGrab = other.gameObject;
            canGrab = true;
        }
        else if (other.CompareTag("Trashbin") && objectToGrab != null)
        {
            UpdatePromptText("Press Q to throw object");
            isNearTrashbin = true;
            trashBinScript = other.gameObject.GetComponent<PlayEffectsTrashbin>();
            // Mostrar el popup
            if (keyPromptUI != null)
                keyPromptUI.SetActive(true);
        }
    }

    #region ThrowOrSave
    private void ThrowObject()
    {
        if (objectToGrab != null && _objectType != null)
        {
            bool isImportant = _objectType.isImportantObject;
            MinigameSevenManager.Instance.TrackObjectProcessed(isImportant, false);
            objectGrabbed = false;
            Destroy(objectToGrab);
            objectToGrab = null;
            _objectType = null;
            UpdatePromptText("");
        }
    }

    private void SaveObject()
    {
        if (objectToGrab != null && _objectType != null)
        {
            bool isImportant = _objectType.isImportantObject;
            MinigameSevenManager.Instance.TrackObjectProcessed(isImportant, true);
            objectGrabbed = false;
            
            Destroy(objectToGrab);
            objectToGrab = null;
            _objectType = null;
            UpdatePromptText("");
        }
    }
    private IEnumerator ResetSaveFlag()
    {
        yield return new WaitForSeconds(0.5f);
        canSave = true;
    }
    #endregion

}
