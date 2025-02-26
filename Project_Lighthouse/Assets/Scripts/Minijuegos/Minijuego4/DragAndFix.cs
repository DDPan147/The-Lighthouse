using System;
using System.Collections;
using UnityEngine;

public class MesaReparacion : MonoBehaviour
{
    public PlayerMovementFP player;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private Transform playerCameraParent;
    public Camera posicionCamara;
    public Camera playerCamera;
    public Transform[] patas; // Array de patas
    public Transform[] posicionesFinales; // Posiciones donde deben ir las patas
    public Transform[] posicionesCamara; // Posiciones espec�ficas de la c�mara
    public Transform camara; // Referencia a la c�mara principal
    [SerializeField] Vector3 pataOffset;
    public float velocidadMovimiento = 2f;
    public float velocidadCamara = 2f;
    private int pataActual = 0;
    private int camaraActual = 0;
    [SerializeField] private bool reparandoObjeto = false;
    private bool fixParte = false;
    private bool camaraPosicionada = false;

    private void Start()
    {
        player = GameObject.Find("Player_Grand").GetComponent<PlayerMovementFP>();
        if (playerCamera != null)
        {
            playerCameraParent = playerCamera.transform.parent;
            originalCameraPosition = playerCamera.transform.localPosition;
            originalCameraRotation = playerCamera.transform.localRotation;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Mesa clickeada"); // Para depuraci�n
        // Cambiar la posici�n de la c�mara
        ChangeToFixObjectCamera();
        if (!reparandoObjeto && camaraPosicionada && pataActual < patas.Length)
        {
            Debug.Log("Moviendo pata " + pataActual);
            StartCoroutine(MoverPata(patas[pataActual], posicionesFinales[pataActual]));
            pataActual++;
        }

        if (pataActual >= patas.Length && !reparandoObjeto)
        {
            ChangeToPlayerCamera();
        }

    }

    IEnumerator MoverPata(Transform pata, Transform destino)
    {
        reparandoObjeto = true;
        while (Vector3.Distance(pata.position, destino.position) > 0.01f)
        {
            fixParte = true;
            pata.position = Vector3.MoveTowards(pata.position, destino.position, velocidadMovimiento * Time.deltaTime);
            yield return null;
        }
        pata.position = destino.position + pataOffset;
        Debug.Log("Pata colocada en su posici�n");
        reparandoObjeto = false;
    }

    void ChangeToFixObjectCamera()
    {
        if (posicionesCamara.Length == 0 || camara == null) return;

        // Primero desactivamos el movimiento del jugador
        player.canMove = false;
    
        // Aseguramos que las cámaras cambien correctamente
        playerCamera.gameObject.SetActive(false);
        posicionCamara.gameObject.SetActive(true);

        camaraActual = (camaraActual + 1) % posicionesCamara.Length;
        StartCoroutine(MoveCameraFixPosition(posicionesCamara[camaraActual]));
    }

    void ChangeToPlayerCamera()
    {
        // Desactivar la cámara de reparación
        posicionCamara.gameObject.SetActive(false);
        
        // Restaurar la posición original de la cámara del jugador
        if (playerCamera != null && playerCameraParent != null)
        {
            playerCamera.transform.parent = playerCameraParent;
            playerCamera.transform.localPosition = originalCameraPosition;
            playerCamera.transform.localRotation = originalCameraRotation;
        }
        
        // Activar la cámara del jugador
        playerCamera.gameObject.SetActive(true);
        
        // Reactivar el movimiento del jugador
        player.canMove = true;
        
        // Resetear estados
        camaraPosicionada = false;
    }

    IEnumerator MoveCameraFixPosition(Transform destino)
    {
        while (Vector3.Distance(camara.position, destino.position) > 0.01f)
        {
            camara.position = Vector3.Lerp(camara.position, destino.position, velocidadCamara * Time.deltaTime);
            camara.rotation = Quaternion.Slerp(camara.rotation, destino.rotation, velocidadCamara * Time.deltaTime);
            yield return null;
        }
        camara.position = destino.position;
        camara.rotation = destino.rotation;
        camaraPosicionada = true;
    }
}
