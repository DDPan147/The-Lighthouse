using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Knife : MonoBehaviour
{
    public bool isCutting;
    [HideInInspector] public bool thereIsFood;
    [HideInInspector] public bool feedbackSupervisor = true;
    [HideInInspector] public GameObject Comida;
    private Selectable_MG2 objData;


    private void Start()
    {
        objData = GetComponent<Selectable_MG2>();
    }

    private void Update()
    {

    }

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


    

}
