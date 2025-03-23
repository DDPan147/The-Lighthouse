using UnityEngine;

public class GearSlot : MonoBehaviour
{
    public int slotPosition;
    private GameObject objectSlot;
    public Transform posSlot;
    public bool isOccupied = false;
    public ClockGear currentGear;
    private ClockManager clockManager;

    private void Awake()
    {
        objectSlot = transform.GetChild(0).gameObject;
        posSlot = objectSlot.GetComponent<Transform>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gear"))
        {
            isOccupied = true;
            currentGear = other.GetComponent<ClockGear>();
            
            if (clockManager != null)
            {
                clockManager.CheckCompletion();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gear"))
        {
            isOccupied = false;
            currentGear = null;
            
            if (clockManager != null)
            {
                clockManager.CheckCompletion();
            }
        }
    }
}
