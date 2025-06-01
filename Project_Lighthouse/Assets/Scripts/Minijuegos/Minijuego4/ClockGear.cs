using DG.Tweening;
using System.Collections;
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
    private Rigidbody rb;
    private Collider gearCollider;
    
    [Header("Material detection")]
    public Material outlineMaterial;
    private MeshRenderer meshRenderer;
    
    [Header("Drag Variables")]
    private Vector3 mousePos;
    [SerializeField] private float zDepth;
    [SerializeField] private bool isOnMouse;
    public bool isDragging = false;
    
    [Header("Efectos")]
    [SerializeField] private float dragSpeed = 10f;
    [SerializeField] private float pickupScale = 1.1f;
    private Vector3 originalScale;
    [SerializeField] private ParticleSystem sparkEffect;
    
    [Header("Animación")]
    [SerializeField] private float snapDuration = 0.3f;
    [SerializeField] private Ease snapEase = Ease.OutBack;
    
    [Header("Referencias")]
    [SerializeField] private GearSlot currentSlot;
    [SerializeField] private Transform slotPos;
    public Camera playerCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        outlineMaterial = GetComponent<MeshRenderer>().materials[1];
        rb.isKinematic = true;
        rb.useGravity = false;
        initialPosition = transform.position;
        originalScale = transform.localScale;
        SetOutlineVisibility(false);
        gearCollider = GetComponent<Collider>();
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

        // Activar física solo para detección de triggers durante el arrastre
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll; // Congelar todo movimiento
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        isOnMouse = false;
        // Mantener configuración para permitir detección
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        StickGearToSlot();
        if (currentSlot != null && currentSlot.slotPosition != correctSlotPosition)
        {
            currentSlot = null;
        }
    }

    private void StickGearToSlot()
    {
        if (currentSlot != null && currentSlot.slotPosition == correctSlotPosition)
        {
            AnimateToPosition(slotPos.position);
            outlineMaterial.SetColor("_Color", Color.green);

            currentSlot.SetGearInSlot(this);
            gearCollider.enabled = false; // Desactivar colisión para evitar más interacciones
            currentSlot.meshMat.enabled = false;

            StartCoroutine(DelayedCompletion());
            StartGearRotation();
            PlaySparkEffect(slotPos.position);

        }
        else
        {
            AnimateToPosition(initialPosition);
            SetOutlineVisibility(false);
            if (currentSlot != null)
            {
                currentSlot.ClearSlot();
            }
        }
    }

    private void StartGearRotation()
    {
        float rotationDirection = (correctSlotPosition % 2 == 0) ? 1f : -1f;
    
        // Cancelar cualquier rotación anterior
        transform.DOKill();
    
        // Rotar 360 grados en la dirección determinada durante 2 segundos
        transform.DORotate(new Vector3(0, 0, 360f * rotationDirection), 2f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                // Opcional: Hacer algo cuando termine la rotación
                Debug.Log($"Engranaje {gearID} completó su rotación");
            });
        rb.constraints = RigidbodyConstraints.FreezeAll; // Congelar el Rigidbody para evitar movimiento
    }

    private void AnimateToPosition(Vector3 targetPosition)
    {
        transform.DOKill();
        transform.DOMove(targetPosition, snapDuration)
            .SetEase(snapEase)
            .OnComplete(() => {
                transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f) * 0.1f, 0.1f, 1, 0.5f);
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
    
    private void PlaySparkEffect(Vector3 position)
    {
        if (sparkEffect != null)
        {
            // Instanciar el efecto en la posición correcta
            ParticleSystem sparkInstance = Instantiate(sparkEffect, position, Quaternion.identity);
            
            // Orientar las partículas hacia arriba o en la dirección deseada
            sparkInstance.transform.rotation = Quaternion.Euler(-90, 0, 0); // Ajusta según la orientación necesaria
            
            // Reproducir el efecto
            sparkInstance.Play();
            
            // Destruir el sistema de partículas después de que termine
            Destroy(sparkInstance.gameObject, sparkInstance.main.duration + 0.5f);
        }
    }

    private IEnumerator DelayedCompletion()
    {
        yield return new WaitForSeconds(snapDuration + 0.1f); // Esperar a que termine la animación
        ClockManager.Instance.CheckCompletion();
    }
}
