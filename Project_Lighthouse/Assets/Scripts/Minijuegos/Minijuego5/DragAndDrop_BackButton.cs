using System;
using UnityEngine;

public class DragAndDrop_Cable: MonoBehaviour
{
    public enum TypeDragDrop
    {
        NoResetPos, // Se puede poner en cualquier slot disponible del minijuego
        ResetPos // Si no accede al slot correcto, este volvera a la posicion de origen
    }

    [Header("Ajustes posicion")]
    public TypeDragDrop type;
    public int correctSlotPosition;
    public int correctOrder;
    private Vector3 initialPosition;
    public float detectionRadius = 1f;

    private bool isConnected;
    private GameManager_Minijuego5 gm;

    [Header("Material detection")]
    public Material outlineMaterial;
    private MeshRenderer meshRenderer;

    [Space]
    private Vector3 mousePos;
    [SerializeField] private float zDepth; // Profundidad del objeto en la camara
    [Header("Script detection")]
    [SerializeField] private DragAndDrop_Connection currentSlot;


    private void Awake()
    {
        meshRenderer = transform.GetChild(1).GetComponent<MeshRenderer>();
        initialPosition = transform.position;
        SetMeshVisibility(true);

        gm = FindFirstObjectByType<GameManager_Minijuego5>();
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
            outlineMaterial.SetColor("_Color", outlineColor);
            outlineMaterial.SetFloat("_Scale", outlineScale);
        }
    }

    private void SetMeshVisibility(bool isVisible)
    {
        /*if(meshRenderer != null)
        {
            meshRenderer.enabled = isVisible;
        }*/
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
                Debug.Log("Tipo reset encajando en el slot correcto");
                transform.position = currentSlot.transform.position - Vector3.forward * 0.5f;
                SetMeshVisibility(false); // Muestra el mesh
                gm.cableConnections[correctSlotPosition] = true;

                if(gm.CurrentCableConnections() == correctOrder)
                {
                    gm.cableCorrectConnections[correctSlotPosition] = true;
                }
                else
                {
                    gm.cableCorrectConnections[correctSlotPosition] = false;
                }
            }
            else
            {
                Debug.Log("No se encontro un slot correcto, regresando a la posicion inicial");
                ResetPosition();
            }
        }
        else
        {
            if (currentSlot != null && type == TypeDragDrop.NoResetPos)
            {
                Debug.Log("Tipo no reset encajando en el slot correcto");
                transform.position = currentSlot.transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("He chocado con algo");
        DragAndDrop_Connection slot = other.GetComponent<DragAndDrop_Connection>();
        if (slot != null)
        {
            if (correctSlotPosition == slot.slotPosition)
            {
                Debug.Log("Posicion correcta, puedes proceder a soltar el objeto");
                currentSlot = slot; // Marca que el objeto esta en el slot correcto
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DragAndDrop_Connection slot = other.GetComponent<DragAndDrop_Connection>();
        if (slot != null)
        {
            SetMeshVisibility(true); // Oculta el outline
            currentSlot = null; // Ya no esta en el slot correcto
        }
    }

    private void OnMouseDrag()
    {
        // Actualiza la posicion del objeto manteniendo la profundidad z original
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        Vector3 newWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos - new Vector3(mousePos.x, mousePos.y, 0));

        Vector3 parentLimit = transform.parent.parent.parent.position;
        transform.position = new Vector3(Mathf.Clamp(newWorldPos.x, parentLimit.x - 5, parentLimit.x + 5), newWorldPos.y, Mathf.Clamp(newWorldPos.z, parentLimit.z - 4, parentLimit.z + 5));

        

        gm.cableConnections[correctSlotPosition] = false;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition; // Vuelve a la posicion inicial si no es correcta
    }
}
