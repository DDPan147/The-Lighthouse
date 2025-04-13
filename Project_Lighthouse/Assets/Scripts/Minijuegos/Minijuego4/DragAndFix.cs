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
    
    [Header("Estados")]
    [SerializeField] private bool repairingObject = false;
    private bool fixingPart = false;
    [SerializeField] private bool hasCheckedInventory = false;

    private void Start()
    {
        playerMovement = GameObject.Find("Player_Grand").GetComponent<PlayerMovementFP>();
        playerCameraScript = GameObject.Find("Player_Grand").GetComponent<CameraController>();
        if (playerCamera != null)
        {
            playerCameraParent = playerCamera.transform.parent;
            originalCameraPosition = playerCamera.transform.localPosition;
            originalCameraRotation = playerCamera.transform.localRotation;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Mesa clickeada");
        
        // Verificar si tenemos suficientes piezas antes de entrar en modo reparación
        if (!hasCheckedInventory)
        {
            if (MinigameFourManager.Instance.HasEnoughParts(ItemType.TableLeg, requiredTableLegs))
            {
                hasCheckedInventory = true;
                playerCameraScript.UnlockCursor();
                ChangeToFixObjectCamera();
            }
            else
            {
                ShowInsufficientPartsMessage();
                return;
            }
        }
        else if (isCamerainPosition && !repairingObject && actualLeg < legs.Length)
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
        repairingObject = false;
        
        // Actualizar feedback cuando se completa una pata
        if (feedbackText != null && actualLeg >= legs.Length)
        {
            feedbackText.text = "¡Mesa reparada!";
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
}
