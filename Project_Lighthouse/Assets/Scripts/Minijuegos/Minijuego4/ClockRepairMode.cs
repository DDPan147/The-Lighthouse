using System.Collections;
using UnityEngine;

public class ClockRepairMode : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    public PlayerMovementFP playerMovement;
    public CameraController playerCameraScript;
    public Camera playerCamera;
    
    [Header("Referencias del Manager")]
    public ClockManager clockManager;
    
    [Header("Referencias de Cámara")]
    public Camera repairCamera;
    private bool isRepairing = false;

    private void Start()
    {
        // Buscar referencias si no están asignadas
        if (playerMovement == null)
            playerMovement = GameObject.Find("Player_Grand").GetComponent<PlayerMovementFP>();
        
        if (playerCameraScript == null)
            playerCameraScript = GameObject.Find("Player_Grand").GetComponent<CameraController>();

        // Asegurarse de que la cámara de reparación esté desactivada al inicio
        if (repairCamera != null)
            repairCamera.gameObject.SetActive(false);
        
        if (clockManager == null)
            clockManager = GetComponentInChildren<ClockManager>();
    }

    private void Update()
    {
        // Salir del modo reparación con Escape
        if (isRepairing && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitRepairMode();
        }
    }

    void OnMouseDown()
    {
        if (!isRepairing)
        {
            EnterRepairMode();
        }
    }

    void EnterRepairMode()
    {
        isRepairing = true;
        playerCameraScript.UnlockCursor();
        ChangeToRepairCamera();
    }

    void ExitRepairMode()
    {
        isRepairing = false;
        ChangeToPlayerCamera();
    }
    
    
    void ChangeToRepairCamera()
    {
        if (repairCamera == null) return;

        // Desactivar movimiento del jugador
        playerMovement.canMove = false;
    
        // Cambiar cámaras
        playerCamera.gameObject.SetActive(false);
        repairCamera.gameObject.SetActive(true);
    }

    void ChangeToPlayerCamera()
    {
        // Cambiar cámaras
        repairCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        
        // Restaurar control del jugador
        playerCameraScript.LockCursor();
        playerMovement.canMove = true;
    }
    public void OnClockFixed()
    {
        if (isRepairing)
        {
            Debug.Log("OnClockFixed");
            StartCoroutine(CompleteRepairSequence());
        }
    }

    IEnumerator CompleteRepairSequence()
    {
        yield return new WaitForSeconds(1f); // Espera para ver la animación
        ExitRepairMode();
    }
}
