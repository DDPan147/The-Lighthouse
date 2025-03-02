using System;
using System.Collections;
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
    
    [Header("Estados")]
    [SerializeField] private bool repairingObject = false;
    private bool fixingPart = false;

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
        playerCameraScript.UnlockCursor();
        ChangeToFixObjectCamera();
        if (!repairingObject && isCamerainPosition && actualLeg < legs.Length)
        {
            Debug.Log("Moviendo pata " + actualLeg);
            StartCoroutine(MoveLeg(legs[actualLeg], finalPositions[actualLeg]));
            actualLeg++;
        }

        if (actualLeg >= legs.Length && !repairingObject)
        {
            ChangeToPlayerCamera();
        }

    }

    IEnumerator MoveLeg(Transform pata, Transform destino)
    {
        repairingObject = true;
        while (Vector3.Distance(pata.position, destino.position) > 0.01f)
        {
            fixingPart = true;
            pata.position = Vector3.MoveTowards(pata.position, destino.position, movementSpeed * Time.deltaTime);
            yield return null;
        }
        pata.position = destino.position + legOffset;
        repairingObject = false;
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
