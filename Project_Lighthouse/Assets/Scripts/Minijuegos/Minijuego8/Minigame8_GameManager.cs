using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;

public class Minigame8_GameManager : MonoBehaviour
{
    private Camera cam;
    public GameObject canica, candado;
    private float rotation;
    public bool canRotate = true;
    void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Win();
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction * 10, out hit) && canRotate)
            {
                if (hit.collider.gameObject.CompareTag("Tuberia"))
                {
                    canRotate = false;
                    rotation += 90;
                    transform.DORotate(new Vector3(candado.transform.localEulerAngles.x,candado.transform.localEulerAngles.y, rotation), 0.5f, RotateMode.Fast).OnComplete(() =>
                    {
                        canica.GetComponent<Pelota>().MoveMarble();
                        StartCoroutine(CanRotate());
                    });
                }
                if (hit.collider.gameObject.CompareTag("Tuberia_Entrada"))
                {
                    canRotate = false;
                    rotation += -90;
                    transform.DORotate(new Vector3(candado.transform.localEulerAngles.x,candado.transform.localEulerAngles.y, rotation), 0.5f, RotateMode.Fast).OnComplete(() =>
                    {
                        canica.GetComponent<Pelota>().MoveMarble();
                        StartCoroutine(CanRotate());
                    });
                }
            }
        }
        
    }

    public void Win()
    {
        Debug.Log("Has Ganado");
        candado.transform.DORotate(new Vector3(0, 90, 0), 1.5f, RotateMode.Fast);

        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            FindAnyObjectByType<GameManager>().MinigameCompleted(7);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }
    }

    public IEnumerator CanRotate()
    {
        yield return new WaitForSeconds(0.5f);
        canRotate = true;
    }
}
