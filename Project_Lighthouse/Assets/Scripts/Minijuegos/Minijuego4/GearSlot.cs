using System.Collections;
using UnityEngine;

public class GearSlot : MonoBehaviour
{
    public int slotPosition;
    private GameObject objectSlot;
    public Transform posSlot;
    public bool isOccupied = false;
    public ClockGear currentGear;
    private ClockManager clockManager;
    public MeshRenderer meshMat;

    private void Awake()
    {
        objectSlot = transform.GetChild(0).gameObject;
        posSlot = objectSlot.GetComponent<Transform>();
        // Buscar el ClockManager en la jerarquía
        clockManager = GetComponentInParent<ClockManager>();
        if (clockManager == null)
        {
            clockManager = FindObjectOfType<ClockManager>();
        }
        if (meshMat == null)
        {
            meshMat = GetComponent<MeshRenderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gear"))
        {
            ClockGear gear = other.GetComponent<ClockGear>();
            if (gear != null)
            {
                Debug.Log($"Gear {gear.gearID} entró en slot {slotPosition}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gear"))
        {
            ClockGear gear = other.GetComponent<ClockGear>();
            if (gear == currentGear && !gear.isDragging)
            {
                Debug.Log($"Gear {gear.gearID} salió del slot {slotPosition}");
                isOccupied = false;
                currentGear = null;

                if (clockManager != null)
                {
                    clockManager.CheckCompletion();
                }
            }
        }
    }
    public void SetGearInSlot(ClockGear gear)
    {
        isOccupied = true;
        currentGear = gear;
        Debug.Log($"Slot {slotPosition} ahora tiene el gear {gear.gearID}");

        if (clockManager != null)
        {
            clockManager.CheckCompletion();
        }
    }

    public void ClearSlot()
    {
        isOccupied = false;
        currentGear = null;
        Debug.Log($"Slot {slotPosition} fue limpiado");

        if (clockManager != null)
        {
            clockManager.CheckCompletion();
        }
    }

    private IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(0.1f); // Pequeño delay
        if (clockManager != null)
        {
            clockManager.CheckCompletion();
        }
    }
}
