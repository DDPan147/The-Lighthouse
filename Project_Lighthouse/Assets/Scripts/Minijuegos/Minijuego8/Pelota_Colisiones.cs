using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class Pelota_Colisiones : MonoBehaviour
{
    private Rigidbody rb;
    [HideInInspector] public bool haGanado;
    private Minigame8_GameManager candadoScript;
    public Transform dropPosition1, dropPosition2;
    public GameObject llave;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        candadoScript = FindAnyObjectByType<Minigame8_GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            //candadoScript.canRotate = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Final")
        {   
            //Ganas el Minijuego
            haGanado = true;
            candadoScript.mc.DisplayComment(4);
            Sequence WinSequence = DOTween.Sequence();
            WinSequence.Append(transform.DOMove(dropPosition1.position, 0.25f));
            WinSequence.Append(transform.DOMove(dropPosition2.position, 0.25f));
            WinSequence.OnComplete(() => StartCoroutine(CompleteMinigame(4f)));
            
        }
    }

    public IEnumerator CompleteMinigame(float seconds)
    {
        candadoScript.marco.transform.DOLocalRotate(new Vector3(candadoScript.marco.transform.localEulerAngles.x, candadoScript.marco.transform.localEulerAngles.y + 90, candadoScript.marco.transform.localEulerAngles.z), 1.5f, RotateMode.Fast).OnComplete(() => llave.GetComponent<VisualEffect>().Play());
        yield return new WaitForSeconds(seconds);
        candadoScript.Win();
    }
}
