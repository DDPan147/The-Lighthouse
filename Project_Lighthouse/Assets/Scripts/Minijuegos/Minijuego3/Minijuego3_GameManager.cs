using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minijuego3_GameManager : MonoBehaviour
{
    private Camera cam;
    private Tuberia[] tuberias;
    public GameObject QueEsEsto;
    void Start()
    {
        cam = Camera.main;
        tuberias = GetComponentsInChildren<Tuberia>();
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
            if (hit.collider.gameObject.CompareTag("Tuberia"))
            {
                float rotation = hit.collider.gameObject.GetComponent<Tuberia>().initialRotation += 90f;
                //hit.collider.gameObject.transform.eulerAngles += new Vector3(0, 0, 90);
                hit.collider.gameObject.transform.DORotate(new Vector3(0, 0, rotation), 0.5f, RotateMode.Fast).OnComplete(() => CheckPipes());
            }
        }
    }

    public void CheckPipes()
    {
        
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
            QueEsEsto.transform.DOMove(Camera.main.transform.position, 1f);
        }
    }
}
