using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Comida_Cortada : MonoBehaviour
{
    public bool isGrabbed;
    [HideInInspector] public bool isCutted;
    [HideInInspector] public bool isRebozado;
    [HideInInspector] public Vector2 moveDirection;
    [HideInInspector] public Material comidaMat;
    public GameObject comida_Cortada;
    private float moveSpeed = 0.4f;
    private bool thereIsBread;
    
    private GameObject rebozadoObj;

    private void Awake()
    {
        rebozadoObj = transform.Find("Rebozado").gameObject;
    }

    void Update()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y);
        if(isRebozado)
        {
            //comidaMat = GetMaterial();
            //comidaMat.color = Color.yellow;
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

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isGrabbed)
        {
            moveDirection = context.ReadValue<Vector2>();
            moveDirection = moveDirection * Time.deltaTime * moveSpeed;
        }
    }

    public void OnCut(InputAction.CallbackContext context)
    {
        if (context.performed && thereIsBread && isGrabbed)
        {
            isRebozado = true;
        }
    }

    /*public Material GetMaterial()
    {
        Material mat;
        mat = GetComponentInChildren<Renderer>().material;
        return mat;
    }*/
}
