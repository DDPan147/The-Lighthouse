using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minijuego3_GameManager : MonoBehaviour
{
    private Camera cam;
    private Tuberia[] tuberias;
    public GameObject QueEsEsto;
    private bool canRotate = true;
    void Start()
    {
        cam = Camera.main;
        //tuberias = GetComponentsInChildren<Tuberia>();
        tuberias = GameObject.FindObjectsByType<Tuberia>(FindObjectsSortMode.None);
        Debug.Log(tuberias.Length);
    }


    void Update()
    {
        
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray.origin, ray.direction * 10, out hit) && context.performed)
        {
            if (hit.collider.gameObject.CompareTag("Tuberia") && canRotate)
            {
                canRotate = false;
                float rotation = hit.collider.gameObject.transform.eulerAngles.z;
                rotation += 90;
                //hit.collider.gameObject.transform.eulerAngles += new Vector3(0, 0, 90);
                hit.collider.gameObject.transform.DORotate(new Vector3(0, 0, rotation), 1.25f, RotateMode.Fast).OnComplete(() => CheckPipes());
            }
        }
    }

    public void CheckPipes()
    {
        canRotate = true;
        int correctPipes = 0;
        for (int i = 0; i < tuberias.Length; i++)
        {
            if (tuberias[i].isConnected)
            {
                correctPipes++;
            }
        }
        if (correctPipes == tuberias.Length)
        {
            //Ha ganado el minijuego
            Debug.Log("Todo correcto");
            QueEsEsto.transform.DOMove(Camera.main.transform.position, 0.5f);
        }
    }
}
