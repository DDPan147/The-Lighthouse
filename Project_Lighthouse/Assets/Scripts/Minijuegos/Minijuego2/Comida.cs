using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Comida : MonoBehaviour
{
    public enum TipoComida
    {
        Patata,
        Zanahoria,
        Pescado,
        RestosPescado
    }
    public TipoComida tipoComida;
    private Selectable_MG2 objData;
    [HideInInspector] public bool isCutted;
    [HideInInspector] public bool canBeCutted;
    [HideInInspector] public bool isRebozado;
    [HideInInspector] public bool isPelado;
    [HideInInspector] public bool canBePelado;
    [HideInInspector] public Material comidaMat;
    public GameObject comida_Cortada;
    [Tooltip("Null if doesn't has")]public GameObject comida_Pelada;
    private float moveSpeed = 0.4f;
    [HideInInspector] public bool thereIsBread;
    [HideInInspector] public bool feedbackSupervisor = true;

    private GameObject rebozadoObj;

    private void Awake()
    {
        rebozadoObj = transform.Find("Rebozado").gameObject;
        objData = GetComponent<Selectable_MG2>();
    }

    void Update()
    {
        if(isRebozado)
        {
            rebozadoObj.SetActive(true);
        }
        else
        {
            rebozadoObj.SetActive(false);
        }

        switch(tipoComida)
        {
            case TipoComida.Patata:
                if(isPelado)
                {
                    canBeCutted = true;
                }
                else
                {
                    canBePelado = true;
                }
                break;
            case TipoComida.Zanahoria:
                if (isPelado)
                {
                    canBeCutted = true;
                }
                else
                {
                    canBePelado = true;
                }
                break;
            case TipoComida.Pescado:
                canBeCutted = true;
                canBePelado = false;
                break;
            case TipoComida.RestosPescado:
                canBeCutted = false;
                canBePelado = false;
                break;
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
