using UnityEngine;
using UnityEngine.InputSystem; 

public class CleaningTool : MonoBehaviour
{
    public enum ToolType { Mop, Hands }
    public ToolType currentTool;

    [Header("Interaction Ranges")]
    [Tooltip("Maximum distance for cleaning interactions")]
    public float cleaningRange = 2f;
    [Tooltip("Maximum distance for highlighting objects")]
    [SerializeField] private float highlightRange = 2f;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CleanableObject lastHighlightedObject;

    [Header("Grab System")]
    [SerializeField] private bool canGrab;
    [SerializeField] private GameObject objectToGrab;

    private void Update()
    {
        CheckMouseHover();
    }

    public void OnCleanInteraction(InputAction.CallbackContext context)
    {
        TryHandsClean();
    }
    
    private void CheckMouseHover()
    {
        if (TryGetCleanableObject(out CleanableObject cleanable, highlightRange))
        {
            UpdateHighlight(cleanable);
        }
        else
        {
            UpdateHighlight(null);
        }
    }

    #region HandsCleaningMethods

    private void TryHandsClean()
    {
        if (currentTool != ToolType.Hands)
        {
            Debug.Log("Current tool selected: " + currentTool);
            return;
        }

        if (TryGetCleanableObject(out CleanableObject cleanable, cleaningRange))
        {
            Debug.Log(cleanable.name);
            canGrab = true;
            objectToGrab = cleanable.gameObject;
            GrabObject();
        }
        else
        {
            Debug.Log("No cleanable object");
        }
    }

    private void GrabObject()
    {
        if (canGrab && objectToGrab != null)
        {
            Debug.Log("Objeto capturado");
            canGrab = false;
        
            // Obtener el componente CleanableObject
            CleanableObject cleanableObject = objectToGrab.GetComponent<CleanableObject>();
            if (cleanableObject != null)
            {
                // Notificar al MinigameFourManager
                MinigameFourManager.Instance.AddItemToInventory(cleanableObject.itemType);
            
                // Desactivar el objeto en lugar de destruirlo
                objectToGrab.SetActive(false);
            }
        
            objectToGrab = null;
        }
        else
        {
            Debug.Log("No hay objeto para capturar");
        }
    }

    #endregion

    #region RaycastAndHighlightSystem

    private bool TryGetCleanableObject(out CleanableObject cleanable, float range)
    {
        cleanable = null;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            cleanable = hit.collider.GetComponent<CleanableObject>();
            return cleanable != null;
        }
        return false;
    }
    private void UpdateHighlight(CleanableObject newCleanable)
    {
        if (lastHighlightedObject != newCleanable)
        {
            if (lastHighlightedObject != null)
            {
                lastHighlightedObject.SetOutlineVisibility(false);
            }
            
            if (newCleanable != null)
            {
                newCleanable.SetOutlineVisibility(true);
                lastHighlightedObject = newCleanable;
            }
            else
            {
                lastHighlightedObject = null;
            }
        }
    }


    #endregion
}