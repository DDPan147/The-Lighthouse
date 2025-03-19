using System;
using UnityEngine;

public class DragDrop_Item : MonoBehaviour
{
    public enum TypeDragDrop
    {
        NoResetPos, // Se puede poner en cualquier slot disponible del minijuego
        ResetPos // Si no accede al slot correcto, este volvera a la posicion de origen
    }

    [Header("Ajustes posicion")]
    public TypeDragDrop type;
    public int correctSlotPosition;
    private Vector3 initialPosition;
    public float detectionRadius = 1f;
    
    [Header("Material detection")]
    public Material outlineMaterial;
    private MeshRenderer meshRenderer;
    
    [Space]
    private Vector3 mousePos;
    [SerializeField] private float zDepth; // Profundidad del objeto en la camara
    [SerializeField] private bool isOnMouse;

    [Header("Script detection")]
    [SerializeField] private DragDrop_Slot currentSlot;
    [SerializeField] private Transform slotPos;


    private void Awake()
    {
        outlineMaterial = GetComponent<MeshRenderer>().materials[1];
        initialPosition = transform.position;
        SetOutlineVisibility(false);
    }

    private void SetOutlineVisibility(bool isVisible)
    {
        if (outlineMaterial != null)
        {
            // Cambia el color alpha para hacer visible/invisible el outline
            Color outlineColor = outlineMaterial.GetColor("_Color");
            float outlineScale = outlineMaterial.GetFloat("_Scale");
            outlineScale = isVisible ? 1.05f : 0f;
            outlineColor.a = isVisible ? 1f : 0f;
            outlineMaterial.SetColor("_Color", Color.yellow);
            outlineMaterial.SetFloat("_Scale", outlineScale);
        }
    }

    private Vector3 GetMousePos()
    {
        // Captura la posicion actual del objeto en la camara y calcula la profundidad
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        zDepth = screenPos.z; // Guarda la profundidad del objeto
        return screenPos;
    }

    private void OnMouseDown()
    {
        mousePos = Input.mousePosition - GetMousePos(); // Calcula el offset al hacer clic
    }
    private void OnMouseUp()
    {
        isOnMouse = false;
        StickItemSlot();
        currentSlot = null;
    }

    private void StickItemSlot()
    {
        // Si se suelta sobre un slot valido
        if (type == TypeDragDrop.ResetPos)
        {
            if (currentSlot != null && currentSlot.slotPosition == correctSlotPosition)
            {
                transform.position = slotPos.position;
                outlineMaterial.SetColor("_Color", Color.green);
                WindowRestorationManager.Instance.OnFragmentCorrectlyPlaced(currentSlot.slotPosition);
            }
            else
            {
                ResetPosition();
                SetOutlineVisibility(false);
            }
        }
        else
        {
            if (currentSlot != null && type == TypeDragDrop.NoResetPos)
            {
                Debug.Log("Tipo no reset encajando en el slot correcto");
                transform.position = slotPos.position;
                if (currentSlot.slotPosition == correctSlotPosition)
                {
                    outlineMaterial.SetColor("_Color", Color.green);
                    WindowRestorationManager.Instance.OnFragmentCorrectlyPlaced(currentSlot.slotPosition);
                }
                Debug.Log(currentSlot.slotPosition);
            }
            else
            {
                SetOutlineVisibility(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null )
        {
            SetOutlineVisibility(true);
            currentSlot = slot;
            slotPos = slot.posSlot;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null && currentSlot == null && isOnMouse == true)
        {
            SetOutlineVisibility(true);
            currentSlot = slot;
            slotPos = slot.posSlot;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null)
        {
            SetOutlineVisibility(false);
            currentSlot = null;
            slotPos = null;
        }
    }

    private void OnMouseDrag()
    {
        isOnMouse = true;
        // Actualiza la posicion del objeto manteniendo la profundidad z original
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        Vector3 newWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos - new Vector3(mousePos.x, mousePos.y, 0));
        transform.position = newWorldPos;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition; // Vuelve a la posicion inicial si no es correcta
    }
}
