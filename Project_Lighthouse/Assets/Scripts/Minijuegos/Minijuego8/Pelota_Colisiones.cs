using System.Collections;
using UnityEngine;

public class Pelota_Colisiones : MonoBehaviour
{
    private Rigidbody rb;
    [HideInInspector] public bool haGanado;
    private GameObject candado;
    private Minigame8_GameManager candadoScript;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        candado = GetComponentInParent<Pelota>().candado;
        candadoScript = candado.GetComponent<Minigame8_GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Final")
        {
            //Ganas el Minijuego
            haGanado = true;
            candadoScript.mc.DisplayComment(4);
            
            StartCoroutine(CompleteMinigame(4f));
        }
    }

    public IEnumerator CompleteMinigame(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        candado.SendMessage("Win");
    }
}
