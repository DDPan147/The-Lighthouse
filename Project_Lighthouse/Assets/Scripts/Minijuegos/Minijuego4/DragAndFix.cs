using System.Collections;
using UnityEngine;

public class MesaReparacion : MonoBehaviour
{
    public Camera posicionCamara;
    public Camera playerCamera;
    public Transform[] patas; // Array de patas
    public Transform[] posicionesFinales; // Posiciones donde deben ir las patas
    public Transform[] posicionesCamara; // Posiciones específicas de la cámara
    public Transform camara; // Referencia a la cámara principal
    [SerializeField] Vector3 pataOffset;
    public float velocidadMovimiento = 2f;
    public float velocidadCamara = 2f;
    private int pataActual = 0;
    private int camaraActual = 0;
    private bool reparando = false;
    private bool camaraPosicionada = false;

    void OnMouseDown()
    {
        Debug.Log("Mesa clickeada"); // Para depuración
        // Cambiar la posición de la cámara
        CambiarPosicionCamara();
        if (!reparando && camaraPosicionada && pataActual < patas.Length)
        {
            Debug.Log("Moviendo pata " + pataActual);
            StartCoroutine(MoverPata(patas[pataActual], posicionesFinales[pataActual]));
            pataActual++;
        }

    }

    IEnumerator MoverPata(Transform pata, Transform destino)
    {
        reparando = true;
        while (Vector3.Distance(pata.position, destino.position) > 0.01f)
        {
            pata.position = Vector3.MoveTowards(pata.position, destino.position, velocidadMovimiento * Time.deltaTime);
            yield return null;
        }
        pata.position = destino.position + pataOffset;
        Debug.Log("Pata colocada en su posición");
        reparando = false;
    }

    void CambiarPosicionCamara()
    {
        if (posicionesCamara.Length == 0 || camara == null) return;

        playerCamera.gameObject.SetActive(false);
        posicionCamara.gameObject.SetActive(true);

        camaraActual = (camaraActual + 1) % posicionesCamara.Length;
        StartCoroutine(MoverCamara(posicionesCamara[camaraActual]));
    }

    IEnumerator MoverCamara(Transform destino)
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
