using UnityEngine;

public class DragDrop_Item : MonoBehaviour
{
    [Header("Ajustes posicion")]
    public int correctSlotPosition;
    private Vector3 initialPosition;
    public float detectionRadius = 1f;

    private Vector3 mousePos;
    [SerializeField] private float zDepth; // Profundidad del objeto en la c�mara
    [Header("Script detection")]
    [SerializeField] private DragDrop_Slot currentSlot;


    private void Awake()
    {
        initialPosition = transform.position;
    }

    private Vector3 GetMousePos()
    {
        // Captura la posici�n actual del objeto en la c�mara y calcula la profundidad
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
        // Si se suelta sobre un slot v�lido
        if (currentSlot != null && currentSlot.slotPosition == correctSlotPosition)
        {
            Debug.Log("Encajado en el slot correcto");
            transform.position = currentSlot.transform.position; // Encaja el �tem en el slot
        }
        else
        {
            Debug.Log("No se encontr� un slot correcto, regresando a la posici�n inicial");
            ResetPosition(); // Vuelve a la posici�n inicial
        }

        // Resetea la referencia al slot actual
        currentSlot = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null && slot.slotPosition == correctSlotPosition)
        {
            Debug.Log("Posicion correcta, puedes proceder a soltar el objeto");
            currentSlot = slot; // Marca que el objeto est� en el slot correcto
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null && slot == currentSlot)
        {
            Debug.Log("Sali� del slot correcto");
            currentSlot = null; // Ya no est� en el slot correcto
        }
    }

    private void OnMouseDrag()
    {
        // Actualiza la posici�n del objeto manteniendo la profundidad z original
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        Vector3 newWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos - new Vector3(mousePos.x, mousePos.y, 0));
        transform.position = newWorldPos;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition; // Vuelve a la posici�n inicial si no es correcta
    }
}
