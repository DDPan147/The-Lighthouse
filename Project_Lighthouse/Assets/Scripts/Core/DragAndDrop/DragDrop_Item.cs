using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class DragDrop_Item : MonoBehaviour
{
    #region Enums
    public enum TypeDragDrop
    {
        NoResetPos, // Se puede poner en cualquier slot disponible del minijuego
        ResetPos // Si no accede al slot correcto, este volvera a la posicion de origen
    }
    
    public enum Minigame // Minijuego para poder realizar la instancia del minijuego
    {
        Minigame1,
        Minigame6 
    }

    #endregion
    
    [Header("Identificación de Minijuego")]
    [SerializeField] private Minigame currentMinigame;

    [Header("Ajustes posicion")]
    public TypeDragDrop type;
    public int correctSlotPosition;
    private Vector3 initialPosition;
    public float detectionRadius = 1f;
    private bool isDragging = false;

    [Header("Material Outline detection")]
    public Material outlineMaterial;

    [Header("Material Glow detection")]
    public Material glowMaterial;
    [SerializeField] private float correctPlacementIntensity = 3.0f;
    [SerializeField] private float transitionDuration = 1.5f; // Duración de la transición
    private Color originalEmissionColor;
    private float originalEmissionIntensity;
    private Coroutine emissionCoroutine;

    [Space]
    private Vector3 mousePos;
    [SerializeField] private float zDepth; // Profundidad del objeto en la camara
    [SerializeField] private bool isOnMouse;

    [Header("Efectos")]
    [SerializeField] private float hoverHeight = 0.2f;
    [SerializeField] private float dragSpeed = 10f;
    [SerializeField] private AnimationCurve dragCurve;
    [SerializeField] private float pickupScale = 1.1f;
    private Vector3 originalScale;
    
    [Header("Restricciones")]
    [SerializeField] private bool canRotate = false;
    [SerializeField] private Vector2 rotationLimits;
    
    [Header("Animación")]
    [SerializeField] private float snapDuration = 0.3f;
    [SerializeField] private Ease snapEase = Ease.OutBack;
    
    [Header("Script detection")]
    [SerializeField] private DragDrop_Slot currentSlot;
    [SerializeField] private Transform slotPos;


    private void Awake()
    {
        glowMaterial = GetComponent<MeshRenderer>().materials[0];
        outlineMaterial = GetComponent<MeshRenderer>().materials[1];
        originalEmissionColor = glowMaterial.GetColor("_EmissionColor");
        originalEmissionIntensity = CalculateColorIntensity(originalEmissionColor);
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
            // Cambia el color alpha para hacer visible/invisible el outline
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
        // Captura la posicion actual del objeto en la camara y calcula la profundidad
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        zDepth = screenPos.z; // Guarda la profundidad del objeto
        return screenPos;
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth);
        return Camera.main.ScreenToWorldPoint(mouseScreenPos - new Vector3(mousePos.x, mousePos.y, 0));
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
        mousePos = Input.mousePosition - GetMousePosDepth(); // Calcula el offset al hacer clic
    }
    private void OnMouseUp()
    {
        isDragging = false;
        isOnMouse = false;
        StickItemSlot();
        currentSlot = null;
    }

    private void StickItemSlot()
    {
        if (type == TypeDragDrop.ResetPos)
        {
            if (currentSlot != null && currentSlot.slotPosition == correctSlotPosition)
            {
                AnimateToPosition(slotPos.position);
                outlineMaterial.SetColor("_Color", Color.green);
                if (currentMinigame == Minigame.Minigame1)
                {
                    LevelGameManagerMinigame1.Instance.OnFragmentCorrectlyPlaced(currentSlot.slotPosition);
                }
                else
                {
                    WindowRestorationManager.Instance.OnFragmentCorrectlyPlaced(currentSlot.slotPosition);
                }
                IncreaseEmissionIntensity();
            }
            else
            {
                AnimateToPosition(initialPosition);
                SetOutlineVisibility(false);
                if (currentMinigame == Minigame.Minigame1)
                {
                    LevelGameManagerMinigame1.Instance.fadeOutEffect.SetActive(true);
                }
                else
                {
                    WindowRestorationManager.Instance.fadeOutEffect.SetActive(true);;
                }
            }
        }
        else
        {
            if (currentSlot != null && type == TypeDragDrop.NoResetPos)
            {
                AnimateToPosition(slotPos.position);
                if (currentSlot.slotPosition == correctSlotPosition)
                {
                    outlineMaterial.SetColor("_Color", Color.green);
                    if (currentMinigame == Minigame.Minigame1)
                    {
                        LevelGameManagerMinigame1.Instance.OnFragmentCorrectlyPlaced(currentSlot.slotPosition);
                    }
                    else
                    {
                        WindowRestorationManager.Instance.OnFragmentCorrectlyPlaced(currentSlot.slotPosition);
                    }
                    IncreaseEmissionIntensity();
                }
                else
                {
                    if (currentMinigame == Minigame.Minigame1)
                    {
                        LevelGameManagerMinigame1.Instance.fadeOutEffect.SetActive(true);
                    }
                    else
                    {
                        WindowRestorationManager.Instance.fadeOutEffect.SetActive(true);;
                    }
                }
            }
            else
            {
                SetOutlineVisibility(false);
            }
        }
    }
    private void AnimateToPosition(Vector3 targetPosition)
    {
        // Cancela cualquier animación previa
        transform.DOKill();
        
        // Anima la posición
        transform.DOMove(targetPosition, snapDuration)
            .SetEase(snapEase)
            .OnComplete(() => {
                // Opcional: Añade un pequeño efecto de rebote al final
                transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.2f);
            });
    }

    private void OnTriggerEnter(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null )
        {
            SetOutlineVisibility(true);
            currentSlot = slot;
            slotPos = slot.posSlot;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null && currentSlot == null && isOnMouse == true)
        {
            SetOutlineVisibility(true);
            currentSlot = slot;
            slotPos = slot.posSlot;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        DragDrop_Slot slot = other.GetComponent<DragDrop_Slot>();
        if (slot != null)
        {
            SetOutlineVisibility(false);
            currentSlot = null;
            slotPos = null;
        }
    }

    public void ResetPosition()
    {
        transform.position = initialPosition; // Vuelve a la posicion inicial si no es correcta
    }
    #region GlowEffect
    private void IncreaseEmissionIntensity()
    {
        // Habilitar emisión
        glowMaterial.EnableKeyword("_EMISSION");

        // Cancelar cualquier animación previa de emisión
        if (emissionCoroutine != null)
            StopCoroutine(emissionCoroutine);

        // Iniciar la nueva transición
        emissionCoroutine = StartCoroutine(TransitionEmissionIntensity());
    }

    private IEnumerator TransitionEmissionIntensity()
    {
        float startTime = Time.time;
        float endTime = startTime + transitionDuration;

        while (Time.time < endTime)
        {
            // Calcular el progreso de la transición (0 a 1)
            float progress = (Time.time - startTime) / transitionDuration;

            // Aplicar una curva de suavizado (opcional)
            float smoothProgress = Mathf.SmoothStep(0, 1, progress);

            // Interpolar entre la intensidad original y la intensidad objetivo
            float currentIntensity = Mathf.Lerp(originalEmissionIntensity, correctPlacementIntensity, smoothProgress);

            // Calcular el color con la nueva intensidad manteniendo el mismo tono
            Color currentColor = new Color(
                originalEmissionColor.r * currentIntensity / originalEmissionIntensity,
                originalEmissionColor.g * currentIntensity / originalEmissionIntensity,
                originalEmissionColor.b * currentIntensity / originalEmissionIntensity
            );

            // Aplicar el color
            glowMaterial.SetColor("_EmissionColor", currentColor);

            yield return null;
        }

        // Asegurar que terminamos con el valor exacto deseado
        Color finalColor = new Color(
            originalEmissionColor.r * correctPlacementIntensity / originalEmissionIntensity,
            originalEmissionColor.g * correctPlacementIntensity / originalEmissionIntensity,
            originalEmissionColor.b * correctPlacementIntensity / originalEmissionIntensity
        );
        glowMaterial.SetColor("_EmissionColor", finalColor);
    }

    private float CalculateColorIntensity(Color color)
    {
        return (color.r + color.g + color.b) / 3f;
    }
    #endregion
}
