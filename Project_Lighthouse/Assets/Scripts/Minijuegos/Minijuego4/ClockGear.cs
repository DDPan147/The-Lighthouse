using DG.Tweening;
using UnityEngine;

public class ClockGear : MonoBehaviour
{
    public enum TypeDragDrop
    {
        NoResetPos, // Se puede poner en cualquier slot disponible del minijuego
        ResetPos // Si no accede al slot correcto, este volvera a la posicion de origen
    }
    
    [Header("Configuración del Engranaje")]
    public int gearID;
    public TypeDragDrop type = TypeDragDrop.ResetPos;
    public int correctSlotPosition;
    private Vector3 initialPosition;
    
    [Header("Material detection")]
    public Material outlineMaterial;
    private MeshRenderer meshRenderer;
    
    [Header("Drag Variables")]
    private Vector3 mousePos;
    [SerializeField] private float zDepth;
    [SerializeField] private bool isOnMouse;
    private bool isDragging = false;
    
    [Header("Efectos")]
    [SerializeField] private float dragSpeed = 10f;
    [SerializeField] private float pickupScale = 1.1f;
    private Vector3 originalScale;
    
    [Header("Animación")]
    [SerializeField] private float snapDuration = 0.3f;
    [SerializeField] private Ease snapEase = Ease.OutBack;
    
    [Header("Referencias")]
    [SerializeField] private GearSlot currentSlot;
    [SerializeField] private Transform slotPos;
    public Camera playerCamera;

    private void Awake()
    {
        outlineMaterial = GetComponent<MeshRenderer>().materials[1];
        initialPosition = transform.position;
        originalScale = transform.localScale;
        SetOutlineVisibility(false);
    }

    private void Update()
    {
        if (isDragging)
        {
            UpdateDragPosition();
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * pickupScale, Time.deltaTime * dragSpeed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * dragSpeed);
        }
    }

    private void SetOutlineVisibility(bool isVisible)
    {
        if (outlineMaterial != null)
        {
            Color outlineColor = outlineMaterial.GetColor("_Color");
            float outlineScale = outlineMaterial.GetFloat("_Scale");
            outlineScale = isVisible ? 1.05f : 0f;
            outlineColor.a = isVisible ? 1f : 0f;
            outlineMaterial.SetColor("_Color", Color.yellow);
            outlineMaterial.SetFloat("_Scale", outlineScale);
        }
    }

    private Vector3 GetMousePosDepth()
    {
        Vector3 screenPos = playerCamera.WorldToScreenPoint(transform.position);
        zDepth = screenPos.z;
        return screenPos;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        return playerCamera.ScreenToWorldPoint(mouseScreenPos - new Vector3(mousePos.x, mousePos.y, 0));
    }

    private void UpdateDragPosition()
    {
        isOnMouse = true;
        Vector3 targetPos = GetMouseWorldPosition();
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * dragSpeed);
    }

    private void OnMouseDown()
    {
        isDragging = true;
        mousePos = Input.mousePosition - GetMousePosDepth();
    }

    private void OnMouseUp()
    {
        isDragging = false;
        isOnMouse = false;
        StickGearToSlot();
        currentSlot = null;
    }

    private void StickGearToSlot()
    {
        if (currentSlot != null && currentSlot.slotPosition == correctSlotPosition)
        {
            AnimateToPosition(slotPos.position);
            outlineMaterial.SetColor("_Color", Color.green);
            ClockManager.Instance.CheckCompletion();
            StartGearRotation();
        }
        else
        {
            AnimateToPosition(initialPosition);
            SetOutlineVisibility(false);
        }
    }

    private void StartGearRotation()
    {
        // Determinar la dirección de rotación basada en la posición del engranaje
        float rotationSpeed = (correctSlotPosition % 2 == 0) ? 90f : -90f;
        transform.DORotate(new Vector3(0, 0, 360f), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
    }

    private void AnimateToPosition(Vector3 targetPosition)
    {
        transform.DOKill();
        transform.DOMove(targetPosition, snapDuration)
            .SetEase(snapEase)
            .OnComplete(() => {
                transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f);
            });
    }

    private void OnTriggerEnter(Collider other)
    {
        GearSlot slot = other.GetComponent<GearSlot>();
        if (slot != null)
        {
            SetOutlineVisibility(true);
            currentSlot = slot;
            slotPos = slot.posSlot;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GearSlot slot = other.GetComponent<GearSlot>();
        if (slot != null)
        {
            SetOutlineVisibility(false);
            currentSlot = null;
            slotPos = null;
        }
    }
}
