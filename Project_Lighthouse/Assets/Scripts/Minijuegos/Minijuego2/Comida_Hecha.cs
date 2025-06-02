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
    public bool isFirstFood;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.FindAnyObjectByType<Minijuego2_GameManager>();
        objData = GetComponent<Selectable_MG2>();
    }


    void Update()
    {
        if (objData.isGrabbed && thereIsDish && isFirstFood)
        {
            transform.DOMove(dropPoint.position, 0.5f).OnComplete(() => gameManager.finishRecipe1 = true);
            //gameManager.finishRecipe = true;
            objData.moveDirection = Vector2.zero;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameManager.imGrabing = false;
            transform.parent = dropPoint;
            /*Sequence DishFinished = DOTween.Sequence();
            DishFinished.Append(transform.DOMove(dropPoint.position, 0.5f));
            DishFinished.Append(dropPoint.parent.transform.DOMoveZ(-1.5f, 2f, false));*/
            objData.isGrabbed = false;
            objData.canBeGrabbed = false;
        }
        else if(objData.isGrabbed && thereIsDish && !isFirstFood)
        {
            transform.DOMove(dropPoint.position, 0.5f).OnComplete(() => gameManager.finishRecipe2 = true);
            //gameManager.finishRecipe = true;
            objData.moveDirection = Vector2.zero;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameManager.imGrabing = false;
            transform.parent = dropPoint;
            /*Sequence DishFinished = DOTween.Sequence();
            DishFinished.Append(transform.DOMove(dropPoint.position, 0.5f));
            DishFinished.Append(dropPoint.parent.transform.DOMoveZ(-1.5f, 2f, false));*/
            objData.isGrabbed = false;
            objData.canBeGrabbed = false;
        }

        if (objData.isGrabbed)
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
