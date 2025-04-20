using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DollRepair : MonoBehaviour
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
    public Transform[] dollParts;
    public Transform[] finalPositions;
    [SerializeField] Vector3 partOffset;
    public float movementSpeed = 2f;
    private int actualPart = 0;
    
    [Header("Requisitos de Reparación")]
    [SerializeField] private int requiredDollParts = 3;
    [SerializeField] private TextMeshProUGUI feedbackText;
    
    [Header("Estados")]
    [SerializeField] private bool repairingObject = false;
    private bool fixingPart = false;
    [SerializeField] public bool hasCheckedInventory = false;

    private void Start()
    {
        // Buscar referencias si no están asignadas
        if (playerMovement == null)
            playerMovement = GameObject.Find("Player_Grand").GetComponent<PlayerMovementFP>();
            
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
        }
    }
    
    private void Update()
    {
        // Si ya estamos en modo reparación y el usuario hace clic, avanzar en la reparación
        if (isCamerainPosition && !repairingObject && hasCheckedInventory && Input.GetMouseButtonDown(0) && actualPart < dollParts.Length)
        {
            Debug.Log("Moviendo parte " + actualPart);
            StartCoroutine(MovePart(dollParts[actualPart], finalPositions[actualPart]));
            actualPart++;
        }
        else if (actualPart >= dollParts.Length && !repairingObject && hasCheckedInventory && Input.GetMouseButtonDown(0))
        {
            // La muñeca está completamente reparada
            ChangeToPlayerCamera();
            MinigameFourManager.Instance.OnDollFixed();
        }
    }
    
    // Método público para activación automática desde el manager
    public void AutoEnterRepairMode()
    {
        Debug.Log("Intentando entrar en modo reparación de muñeca automáticamente");
        
        // Verificar si el jugador tiene suficientes piezas
        bool tieneSuficientes = MinigameFourManager.Instance.HasEnoughParts(ItemType.DollPart, requiredDollParts);
        Debug.Log($"¿Tiene suficientes piezas? {tieneSuficientes} (Necesita: {requiredDollParts})");
        
        if (tieneSuficientes)
        {
            Debug.Log("Inventario verificado, cambiando a cámara de reparación");
            hasCheckedInventory = true;
            playerCameraScript.UnlockCursor();
            ChangeToFixObjectCamera();
        }
        else
        {
            ShowInsufficientPartsMessage();
        }
    }

    // El método OnMouseDown solo para avanzar en la reparación
    void OnMouseDown()
    {
        // Solo procesar clics si estamos en modo reparación
        if (hasCheckedInventory && isCamerainPosition)
        {
            if (!repairingObject && actualPart < dollParts.Length)
            {
                Debug.Log("Moviendo parte " + actualPart);
                StartCoroutine(MovePart(dollParts[actualPart], finalPositions[actualPart]));
                actualPart++;
            }
            else if (actualPart >= dollParts.Length && !repairingObject)
            {
                ChangeToPlayerCamera();
                MinigameFourManager.Instance.OnDollFixed();
            }
        }
    }

    private void ShowInsufficientPartsMessage()
    {
        if (feedbackText != null)
        {
            int currentParts = MinigameFourManager.Instance.dollPartsCollected;
            feedbackText.text = $"Necesitas {requiredDollParts} piezas para reparar la muñeca. Tienes {currentParts}.";
            
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

    IEnumerator MovePart(Transform parte, Transform destino)
    {
        repairingObject = true;
        
        // Actualizar feedback si existe
        if (feedbackText != null)
        {
            feedbackText.text = $"Reparando muñeca... ({actualPart+1}/{dollParts.Length})";
        }
        
        while (Vector3.Distance(parte.position, destino.position) > 0.01f)
        {
            fixingPart = true;
            parte.position = Vector3.MoveTowards(parte.position, destino.position, movementSpeed * Time.deltaTime);
            yield return null;
        }
        parte.position = destino.position + partOffset;
        repairingObject = false;
        
        // Actualizar feedback cuando se completa una parte
        if (feedbackText != null && actualPart >= dollParts.Length)
        {
            feedbackText.text = "¡Muñeca reparada! Haz clic para continuar.";
        }
    }

    void ChangeToFixObjectCamera()
    {
        Debug.Log("Intentando cambiar a cámara de reparación de muñeca");
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
            feedbackText.text = "Haz clic para comenzar a reparar la muñeca";
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
    
    // Método público para resetear el estado (útil para el MinigameFourManager)
    public void ResetState()
    {
        hasCheckedInventory = false;
        repairingObject = false;
        isCamerainPosition = false;
        actualPart = 0;
        
        if (feedbackText != null)
            feedbackText.text = "";
    }
}