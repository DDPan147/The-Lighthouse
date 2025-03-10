using System.Collections.Generic;
using UnityEngine;

public class DetectarObjetoDebajo : MonoBehaviour
{
    private Camera cam;
    public LayerMask seleccionables;
    private Vector3 lastMousePosition;
    public Material outlineMat;
    private List<Material> outlineMatList = new List<Material>();
    private bool isSelected;
    private GameObject currentObj;
    private Material currentObjMat;
    private void Awake()
    {
        cam = Camera.main;
    }
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.mousePosition != lastMousePosition)
        {
            DetectUnderMouse();
            lastMousePosition = Input.mousePosition;
        }
    }

    public void DetectUnderMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        outlineMatList.Clear();
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, seleccionables))
        {
            currentObj = hit.collider.gameObject;
            currentObjMat = currentObj.GetComponent<Renderer>().material;
            AddOutlineMaterial(currentObj);
        }
        else
        {
            if(currentObj != null)
            {
                RemoveOutlineMaterial(currentObj);
                currentObj = null;
                currentObjMat = null;
            }
        }
    }

    void AddOutlineMaterial(GameObject obj)
    {
        if(!isSelected)
        {
            outlineMatList.Add(currentObjMat);
            outlineMatList.Add(outlineMat);
            obj.GetComponent<Renderer>().SetMaterials(outlineMatList);
            isSelected = true;
        }
    }

    void RemoveOutlineMaterial(GameObject obj)
    {
        if (isSelected)
        {
            outlineMatList.Add(currentObjMat);
            obj.GetComponent<Renderer>().SetMaterials(outlineMatList);
            isSelected = false;
        }
        
    }


}
