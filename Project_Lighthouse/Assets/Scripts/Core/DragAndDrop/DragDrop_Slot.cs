using UnityEngine;

public class DragDrop_Slot : MonoBehaviour
{
    public int slotPosition; // La posicion que este slot representa
    private GameObject objectSlot;
    public Transform posSlot;
    public int taskIndex;
    public MeshRenderer meshMat;

    private void Awake()
    {
        objectSlot = transform.GetChild(0).gameObject;
        //objectSlot = GetComponentInChildren<GameObject>();
        posSlot = objectSlot.GetComponent<Transform>();
        if (meshMat == null)
        {
            meshMat = GetComponent<MeshRenderer>();
        }
    }
}
