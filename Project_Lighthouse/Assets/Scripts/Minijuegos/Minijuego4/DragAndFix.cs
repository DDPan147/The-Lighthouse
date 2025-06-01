using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MesaReparacion : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    public PlayerMovementFP playerMovement;
    public CameraController playerCameraScript;
    public Camera playerCamera;
    private Transform playerCameraParent;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    [Header("Referencias de Cámara")]
    public Camera cameraObject;
    public Transform[] posicionesCamara;
    private int actualCamera = 0;
    [SerializeField] private bool isCamerainPosition = false;

    [Header("Sistema de Reparación")]
    public Transform[] legs;
    public Transform[] finalPositions;
    [SerializeField] Vector3 legOffset;
    public float movementSpeed = 2f;
    private int actualLeg = 0;
    
    [Header("Requisitos de Reparación")]
    [SerializeField] private int requiredTableLegs = 4;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject interactionPromptObject;
    
    [Header("Estados")]
    [SerializeField] private bool repairingObject = false;
    private bool fixingPart = false;
    [SerializeField] public bool hasCheckedInventory = false;
    
    [Header("Interacción")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private bool playerInRange = false;
    [SerializeField] private KeyCode interactionKey = KeyCode.Q;
    
    private Transform playerTransform;

    private void Start()
    {
        // Buscar referencias si no están asignadas
        if (playerMovement != null)
            playerTransform = playerMovement.transform;
            
        if (playerCameraScript == null)
            playerCameraScript = GameObject.Find("Player_Grand").GetComponent<CameraController>();
            
        if (playerCamera == null)
        {
            GameObject cameraObj = GameObject.FindWithTag("MainCamera");
            if (cameraObj != null)
                playerCamera = cameraObj.GetComponent<Camera>();
        }
        
        if (playerCamera != null)
        {
            playerCameraParent = playerCamera.transform.parent;
            originalCameraPosition = playerCamera.transform.localPosition;
            originalCameraRotation = playerCamera.transform.localRotation;
            playerTransform = playerCamera.transform;
        }
        
        // Ocultar prompt de interacción al inicio
        if (interactionPromptObject != null)
            interactionPromptObject.gameObject.SetActive(false);
            
        // Asegurar que hay un collider para la detección
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("La mesa no tiene collider. Añadiendo BoxCollider.");
            gameObject.AddComponent<BoxCollider>();
        }
    }
    
    private void Update()
    {
        // Detectar pulsación de tecla E cuando el jugador está en rango
        // Comprobar si el jugador está en rango para interactuar
        CheckPlayerDistance();
    
        // Debug para comprobar distancia y estado
        if (Input.GetKeyDown(interactionKey))
        {
            Debug.Log($"Tecla {interactionKey} presionada. PlayerInRange: {playerInRange}, hasCheckedInventory: {hasCheckedInventory}, repairingObject: {repairingObject}");
        }
        
        // Detectar pulsación de tecla cuando el jugador está en rango
        if (playerInRange && Input.GetKeyDown(interactionKey) && !hasCheckedInventory && !repairingObject)
        {
            Debug.Log("Activando modo reparación con tecla");
            AttemptToEnterRepairMode();
        }
        
        // Si ya estamos en modo reparación y el usuario hace clic, avanzar en la reparación
        if (isCamerainPosition && !repairingObject && hasCheckedInventory && Input.GetMouseButtonDown(0) && actualLeg < legs.Length)
        {
            Debug.Log("Moviendo pata " + actualLeg);
            StartCoroutine(MoveLeg(legs[actualLeg], finalPositions[actualLeg]));
            actualLeg++;
        }
        else if (actualLeg >= legs.Length && !repairingObject && hasCheckedInventory && Input.GetMouseButtonDown(0))
        {
            // La mesa está completamente reparada
            ChangeToPlayerCamera();
            MinigameFourManager.Instance.OnTableFixed();
        }
    }
    
    private void CheckPlayerDistance()
    {
        if (playerTransform == null) 
        {
            Debug.LogWarning("playerTransform es null en CheckPlayerDistance");
            return;
        }
    
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionDistance;
    
        Debug.Log($"Distancia al jugador: {distance}, En rango: {playerInRange}");
    
        // Si el estado cambió, actualizar UI
        if (wasInRange != playerInRange)
        {
            ShowInteractionPrompt(playerInRange);
        }
    }
    
    private void ShowInteractionPrompt(bool show)
    {
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(show);
        }
    }
    
    public void AttemptToEnterRepairMode()
    {
        Debug.Log("Intentando entrar en modo reparación");
        
        // Verificar si el jugador tiene suficientes piezas
        bool tieneSuficientes = MinigameFourManager.Instance.HasEnoughParts(ItemType.TableLeg, requiredTableLegs);
        Debug.Log($"¿Tiene suficientes patas? {tieneSuficientes} (Necesita: {requiredTableLegs})");
        
        if (tieneSuficientes)
        {
            Debug.Log("Inventario verificado, cambiando a cámara de reparación");
            hasCheckedInventory = true;
            playerCameraScript.UnlockCursor();
            ChangeToFixObjectCamera();
            
            // Ocultar prompt de interacción durante la reparación
            ShowInteractionPrompt(false);
        }
        else
        {
            ShowInsufficientPartsMessage();
        }
    }

    // El método OnMouseDown puede mantenerse como alternativa
    void OnMouseDown()
    {
        // Solo procesar clics si estamos en modo reparación
        if (hasCheckedInventory && isCamerainPosition)
        {
            if (!repairingObject && actualLeg < legs.Length)
            {
                Debug.Log("Moviendo pata " + actualLeg);
                StartCoroutine(MoveLeg(legs[actualLeg], finalPositions[actualLeg]));
                actualLeg++;
            }
            else if (actualLeg >= legs.Length && !repairingObject)
            {
                ChangeToPlayerCamera();
                MinigameFourManager.Instance.OnTableFixed();
            }
        }
    }

    private void ShowInsufficientPartsMessage()
    {
        if (feedbackText != null)
        {
            int currentLegs = MinigameFourManager.Instance.tableLegsCollected;
            feedbackText.text = $"Necesitas {requiredTableLegs} patas para reparar la mesa. Tienes {currentLegs}.";
            
            // Mostrar el mensaje por un tiempo limitado
            StartCoroutine(HideFeedbackAfterDelay(3f));
        }
        else
        {
            Debug.LogWarning("No hay texto de feedback asignado para mostrar el mensaje");
        }
    }
    
    private IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    IEnumerator MoveLeg(Transform pata, Transform destino)
    {
        repairingObject = true;
        
        // Actualizar feedback si existe
        if (feedbackText != null)
        {
            feedbackText.text = $"Reparando mesa... ({actualLeg+1}/{legs.Length})";
        }
        
        while (Vector3.Distance(pata.position, destino.position) > 0.01f)
        {
            fixingPart = true;
            pata.position = Vector3.MoveTowards(pata.position, destino.position, movementSpeed * Time.deltaTime);
            yield return null;
        }
        pata.position = destino.position + legOffset;
        if (SoundManager.instance != null)
        {
            SoundManager.instance.Play("Cortar");
        }
        repairingObject = false;
        
        // Actualizar feedback cuando se completa una pata
        if (feedbackText != null && actualLeg >= legs.Length)
        {
            feedbackText.text = "¡Mesa reparada! Haz clic para continuar.";
        }
    }

    void ChangeToFixObjectCamera()
    {
        Debug.Log("Intentando cambiar a cámara de reparación");
        if (posicionesCamara.Length == 0 || cameraObject == null) return;

        // Primero desactivamos el movimiento del jugador
        playerMovement.canMove = false;
        
        // Aseguramos que las camaras cambien correctamente
        playerCamera.gameObject.SetActive(false);
        cameraObject.gameObject.SetActive(true);
        
        actualCamera = (actualCamera + 1) % posicionesCamara.Length;
        StartCoroutine(SetBoolWaitTime(.1f));
        
        // Actualizar feedback si existe
        if (feedbackText != null)
        {
            feedbackText.text = "Haz clic para comenzar a reparar la mesa";
        }
    }

    void ChangeToPlayerCamera()
    {
        // Desactivar la cámara de reparación
        cameraObject.gameObject.SetActive(false);
        
        // Restaurar la posición original de la cámara del jugador
        if (playerCamera != null && playerCameraParent != null)
        {
            playerCamera.transform.parent = playerCameraParent;
            playerCamera.transform.localPosition = originalCameraPosition;
            playerCamera.transform.localRotation = originalCameraRotation;
        }
        playerCamera.gameObject.SetActive(true);
        playerCameraScript.LockCursor();
        playerMovement.canMove = true;
        isCamerainPosition = false;
    }

    IEnumerator SetBoolWaitTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        isCamerainPosition = true;
    }
    
    // Para visualizar el rango de interacción en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
    
    // Método público para resetear el estado (útil para el MinigameFourManager)
    public void ResetState()
    {
        hasCheckedInventory = false;
        repairingObject = false;
        isCamerainPosition = false;
        actualLeg = 0;
        
        if (feedbackText != null)
            feedbackText.text = "";
    }
}