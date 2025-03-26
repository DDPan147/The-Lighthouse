using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Comida_Hecha : MonoBehaviour
{
    private Minijuego2_GameManager gameManager;
    private Selectable_MG2 objData;
    private bool thereIsDish;
    private Transform dropPoint;
    void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<Minijuego2_GameManager>();
        objData = GetComponent<Selectable_MG2>();
    }


    void Update()
    {
        if (objData.isGrabbed && thereIsDish)
        {
            gameManager.finishRecipe = true;
            transform.DOMove(dropPoint.position, 0.5f);
            objData.moveDirection = Vector2.zero;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            objData.isGrabbed = false;
            gameManager.imGrabing = false;
            transform.parent = dropPoint;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Plato"))
        {
            thereIsDish = true;
            dropPoint = other.gameObject.transform.GetChild(0).transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Plato"))
        {
            thereIsDish = false;
            dropPoint = null;
        }
    }

    

}
