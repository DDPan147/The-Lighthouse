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

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string grabAnimationTrigger = "Grab";

    [Header("Papelera")]
    [SerializeField] private GameObject keyPromptUI;
    private bool isNearTrashbin = false;
    [SerializeField] private PlayEffectsTrashbin trashBinScript;
    
    [Header("Input Control")]
    private bool canSave = true; // Para evitar múltiples guardados
    
    [Header("UI")]
    [SerializeField] private MinigameSevenUI gameUI;
    private void Start()
    {
        playerMovement = GetComponent<Player_Movement_Minigame_7>();
        OnTriggerEnter(detectionCollider);
        if (keyPromptUI != null)
            keyPromptUI.SetActive(false);
        
        if (gameUI == null)
            gameUI = FindAnyObjectByType<MinigameSevenUI>();

        UpdatePromptText("");
    }
    
    private void UpdatePromptText(string message)
    {
        if (_playerText != null)
        {
            _playerText.text = message;
        }
        
        // Actualizar también en la UI
        if (gameUI != null)
        {
            gameUI.UpdateActionPrompt(message);
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
            UpdatePromptText("R para guardar el objeto");
        }
    }
    #region Inputs

    //Suscribimos los inputs
    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed && objectToGrab != null && canGrab)
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
            
            if (gameUI != null)
            {
                gameUI.ShowObjectInfo(typeOb);
            }
        }
        CheckTag(other);
    }
    
    //Y ontriggerexit para poner las variables en default
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Objeto"))
        {
            if (gameUI != null)
            {
                gameUI.HideObjectInfo();
            }
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
    private bool isGrabbing = false; // Nueva variable

    private void GrabObject()
    {
        if (canGrab && objectToGrab != null && !isGrabbing) // Agregar !isGrabbing
        {
            isGrabbing = true; // Bloquear múltiples ejecuciones

            // SOLO activar la animación aquí, NO la lógica de agarre
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(grabAnimationTrigger);
            }

            // Iniciar la corrutina que manejará todo el proceso
            StartCoroutine(GrabObjectWithAnimation());
        }
        else
        {
            Debug.Log("No hay objeto para capturar o ya está agarrando");
        }
    }

    private IEnumerator GrabObjectWithAnimation()
    {
        // Desactivar movimiento durante la animación
        if (playerMovement != null)
        {
            playerMovement.isMovementBlocked = true;
        }

        // Esperar un tiempo para que la animación se reproduzca
        yield return new WaitForSeconds(2.3f);


        if (SoundManager.instance != null)
        {
            SoundManager.instance.Play("Coger");
        }
        yield return new WaitForSeconds(4f);

        // AQUÍ ejecutar la lógica de agarre (solo una vez)
        if (objectToGrab != null)
        {
            objectToGrab.transform.parent = transform;
            objectToGrab.transform.localPosition = _objectPositionOffset;
            Collider collider = objectToGrab.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            canGrab = false;
            objectGrabbed = true;
        }

        // Desbloquear movimiento
        if (playerMovement != null)
        {
            playerMovement.isMovementBlocked = false;
        }

        isGrabbing = false; // Permitir nuevos agarres
    }
    private void CheckTag(Collider other)
    {
        if (other.CompareTag("Objeto"))
        {
            UpdatePromptText("E para coger objeto");
            objectToGrab = other.gameObject;
            canGrab = true;
        }
        else if (other.CompareTag("Trashbin") && objectToGrab != null)
        {
            UpdatePromptText("Q para tirar objeto");
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
        
            if (MinigameSevenManager.Instance != null)
            {
                MinigameSevenManager.Instance.TrackObjectProcessed(isImportant, false);
            }
            else
            {
                Debug.LogError("MinigameSevenManager.Instance es null en ThrowObject()");
            }
        
            if (trashBinScript != null)
            {
                trashBinScript.PlayEffects();
            }
        
            if (gameUI != null)
            {
                gameUI.UpdateObjectCounters();
                gameUI.UpdateEmotionalState();
                gameUI.HideObjectInfo();
            }
        
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
            
            // Actualizar contadores en la UI
            if (gameUI != null)
            {
                gameUI.UpdateObjectCounters();
                gameUI.UpdateEmotionalState();
                gameUI.HideObjectInfo();
            }
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
