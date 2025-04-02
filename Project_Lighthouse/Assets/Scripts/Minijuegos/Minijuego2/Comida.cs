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
    [HideInInspector] public bool canBeRebozado;
    [HideInInspector] public bool isPelado;
    [HideInInspector] public bool canBePelado;
    [HideInInspector] public bool isReady;
    [HideInInspector] public Material comidaMat;
    public GameObject comida_Cortada;
    [Tooltip("Null if it doesn't have")]public GameObject comida_Pelada;
    private float moveSpeed = 0.4f;
    [HideInInspector] public bool thereIsBread;
    [HideInInspector] public bool feedbackSupervisor = true;

    private GameObject rebozadoObj;
    private Rigidbody rb;

    private void Awake()
    {
        rebozadoObj = transform.Find("Rebozado").gameObject;
        objData = GetComponent<Selectable_MG2>();
        rb = GetComponent<Rigidbody>();
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
                canBeRebozado = false;

                if(isPelado && isCutted)
                {
                    isReady = true;
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
                canBeRebozado = false;

                if (isPelado && isCutted)
                {
                    isReady = true;
                }
                break;
            case TipoComida.Pescado:
                canBeCutted = true;
                canBePelado = false;
                canBeRebozado = true;

                if(isCutted && isRebozado)
                {
                    isReady = true;
                }
                break;
            case TipoComida.RestosPescado:
                canBeCutted = false;
                canBePelado = false;
                canBeRebozado = false;
                isReady = true;
                break;
        }

        if(objData.isGrabbed)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
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


}
