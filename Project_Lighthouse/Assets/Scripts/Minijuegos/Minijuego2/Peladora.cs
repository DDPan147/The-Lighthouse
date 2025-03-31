using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Peladora : MonoBehaviour
{
    public bool isPelando;
    //public GameObject Comida_Pelada;
    //[HideInInspector] public Vector3 origPosition;
    private bool thereIsFood;
    private bool feedbackSupervisor = true;
    //public Transform minLimitX, minLimitZ, maxLimitX, maxLimitZ;
    private GameObject Comida;
    //public bool isGrabbed;
    private Selectable_MG2 objData;


    private void Start()
    {
        //origPosition = transform.position;
        objData = GetComponent<Selectable_MG2>();
    }

    private void Update()
    {
        //LimitarMovimiento();
    }
    //void LimitarMovimiento()
    //{
    //    if (transform.position.x < minLimitX.position.x)
    //    {
    //        transform.position = new Vector3(minLimitX.position.x, transform.position.y, transform.position.z);
    //    }
    //    else if (transform.position.x > maxLimitX.position.x)
    //    {
    //        transform.position = new Vector3(maxLimitX.position.x, transform.position.y, transform.position.z);
    //    }
    //    else if (transform.position.z < minLimitZ.position.z)
    //    {
    //        transform.position = new Vector3(transform.position.x, transform.position.y, minLimitZ.position.z);
    //    }
    //    else if (transform.position.z > maxLimitZ.position.z)
    //    {
    //        transform.position = new Vector3(transform.position.x, transform.position.y, maxLimitZ.position.z);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Comida"))
        {
            thereIsFood = true;
            Comida = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Comida"))
        {
            thereIsFood = false;
        }
    }


    public void OnCut(InputAction.CallbackContext context)
    {
        if (context.performed && objData.isGrabbed)
        {
            isPelando = true;
        }
        else if(context.canceled && objData.isGrabbed)
        {
            isPelando = false;
        }
        if (context.performed && thereIsFood && feedbackSupervisor)
        {
            Comida comida_Cortada = Comida.GetComponent<Comida>();
            if (comida_Cortada.canBePelado)
            {
                Destroy(Comida.transform.Find("Forma").gameObject);
                Instantiate(comida_Cortada.comida_Pelada, Comida.transform);
                comida_Cortada.isPelado = true;
                thereIsFood = false;
            }
            else
            {
                // Feedback de que la comida no se puede pelar
                comida_Cortada.gameObject.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => feedbackSupervisor = false).OnComplete(() => feedbackSupervisor = true);
            }
            
        }
    }


}
