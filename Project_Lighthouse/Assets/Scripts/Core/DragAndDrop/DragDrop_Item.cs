using UnityEngine;

public class DragDrop_Item : MonoBehaviour
{
    [Header("Ajustes posicion")]
    public int correctSlotPosition;
    private Vector3 initialPosition;
    public float detectionRadius = 1f;
    
    [Header("Material detection")]
    public Material outlineMaterial;
    private MeshRenderer meshRenderer;
    
    [Space]
    private Vector3 mousePos;
    [SerializeField] private float zDepth; // Profundidad del objeto en la camara
    [Header("Script detection")]
    [SerializeField] private DragDrop_Slot currentSlot;


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
            outlineMaterial.SetColor("_Color", outlineColor);
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
        // Si se suelta sobre un slot valido
        if (currentSlot != null && currentSlot.slotPosition == correctSlotPosition)
        {
            Debug.Log("Encajado en el slot correcto");
            transform.position = currentSlot.transform.position; // Encaja el item en el slot
        }
        else
        {
            Debug.Log("No se encontro un slot correcto, regresando a la posici�n inicial");
            ResetPosition(); // Vuelve a la posicion inicial
        }

        // Resetea la referencia al slot actual
        currentSlot = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null && slot.slotPosition == correctSlotPosition)
        {
            SetOutlineVisibility(true); // Muestra el outline
            Debug.Log("Posicion correcta, puedes proceder a soltar el objeto");
            currentSlot = slot; // Marca que el objeto est� en el slot correcto
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null && slot == currentSlot)
        {
            SetOutlineVisibility(false); // Oculta el outline
            Debug.Log("Sali� del slot correcto");
            currentSlot = null; // Ya no esta en el slot correcto
        }
    }

    private void OnMouseDrag()
    {
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
