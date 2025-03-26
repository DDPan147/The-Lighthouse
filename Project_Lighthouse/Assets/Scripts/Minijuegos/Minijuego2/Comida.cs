using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Comida : MonoBehaviour
{
    //public bool isGrabbed;
    private Selectable_MG2 objData;
    [HideInInspector] public bool isCutted;
    [HideInInspector] public bool isRebozado;
    //[HideInInspector] public Vector2 moveDirection;
    [HideInInspector] public Material comidaMat;
    public GameObject comida_Cortada;
    private float moveSpeed = 0.4f;
    private bool thereIsBread;
    
    private GameObject rebozadoObj;

    private void Awake()
    {
        rebozadoObj = transform.Find("Rebozado").gameObject;
        objData = GetComponent<Selectable_MG2>();
    }

    void Update()
    {
        //transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
        if(isRebozado)
        {
            rebozadoObj.SetActive(true);
        }
        else
        {
            rebozadoObj.SetActive(false);
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PanRallado"))
        {
            thereIsBread = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PanRallado"))
        {
            thereIsBread = false;
        }
    }

    /*public void OnMove(InputAction.CallbackContext context)
    {
        if (isGrabbed)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection = moveDirection * Time.deltaTime * moveSpeed;
        }
    }*/

    public void OnCut(InputAction.CallbackContext context)
    {
        if (context.performed && thereIsBread && objData.isGrabbed && isCutted)
        {
            isRebozado = true;
        }
        else if (context.performed && !isCutted && objData.isGrabbed)
        {
            //Feedback visual de que falta cortarlo/pelarlo
        }
    }

}
