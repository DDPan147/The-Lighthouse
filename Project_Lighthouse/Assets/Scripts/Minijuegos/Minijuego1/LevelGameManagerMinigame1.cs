using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelGameManagerMinigame1 : MonoBehaviour
{
    public static LevelGameManagerMinigame1 Instance { get; private set; }

    [Header("Referencias de la Escena")]
    [SerializeField] private DragDrop_Item[] itemsList;
    [SerializeField] private DragDrop_Slot[] itemSlots;
    [SerializeField] private int[] itemTaskIndices; // Índice de tarea para cada item
    [SerializeField] private int[] slotTaskIndices; // Índice de tarea para cada slot
    public GameObject fadeOutEffect;
    public GameObject fadeOutEffectChangeCamera;
    public Button checkButton;
    private MinigameComments mc;
    
    [Header("Configuración de Cámaras")]
    public Camera[] taskCameras; // Cámaras para cada tarea (cajones, estanterías, cubiertos)
    [SerializeField] public int currentTaskIndex = 0; // Índice de la tarea actual
    
    [Header("Estado del Juego")]
    [SerializeField] private int correctPlacements = 0;
    private bool isMinigameComplete = false;
    private Dictionary<int, bool> slotCompletionStatus;
    
    [Header("Eventos")]
    public UnityEvent onMinigameStart;
    public UnityEvent onMinigameComplete;
    public UnityEvent<int> onFragmentPlaced;
    public UnityEvent onTaskComplete;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        InitializeMinigame();
        mc = GetComponent<MinigameComments>();
    }
    
    private void InitializeMinigame()
    {
        slotCompletionStatus = new Dictionary<int, bool>();
        foreach (var slot in itemSlots)
        {
            slotCompletionStatus[slot.slotPosition] = false;
        }
        ValidateSetup();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab) && Input.GetKeyDown(KeyCode.RightControl))
        {
            // Llamar al GameManager para volver al lobby
            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                gm.MinigameCompleted(0);
                Debug.Log("Llamando a GameManager.MinigameCompleted(0)");
            }
            else
            {
                Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
            }
        }
    }

    private void ValidateSetup()
    {
        if (itemsList == null || itemsList.Length == 0)
        {
            Debug.LogError("WindowRestorationManager: No glass fragments assigned!");
            return;
        }
        if (itemSlots == null || itemSlots.Length == 0)
        {
            Debug.LogError("WindowRestorationManager: No window slots assigned!");
            return;
        }
        if (itemsList.Length != itemSlots.Length)
        {
            Debug.LogWarning("WindowRestorationManager: Mismatch between fragments and slots count!");
        }
        if (taskCameras == null || taskCameras.Length == 0)
        {
            Debug.LogError("No task cameras assigned!");
            return;
        }
        
        // Validar que tenemos índices de tarea para cada item y slot
        if (itemTaskIndices == null || itemTaskIndices.Length != itemsList.Length)
        {
            Debug.LogError("Item task indices array doesn't match items count!");
            return;
        }
        if (slotTaskIndices == null || slotTaskIndices.Length != itemSlots.Length)
        {
            Debug.LogError("Slot task indices array doesn't match slots count!");
            return;
        }
    }

    public void StartMinigame()
    {
        mc.DisplayComment(0);
        correctPlacements = 0;
        isMinigameComplete = false;
        currentTaskIndex = 0;
        
        foreach (var key in slotCompletionStatus.Keys.ToList())
        {
            slotCompletionStatus[key] = false;
        }
        
        // Activar solo la primera cámara
        SwitchToCamera(currentTaskIndex);
        
        // Activar solo los items y slots de la tarea actual
        UpdateActiveItemsAndSlots();
        
        onMinigameStart?.Invoke();
    }

    private void UpdateActiveItemsAndSlots()
    {
        // Activar/desactivar items según la tarea actual
        for (int i = 0; i < itemsList.Length; i++)
        {
            if (itemsList[i] != null)
            {
                bool isCurrentTask = itemTaskIndices[i] == currentTaskIndex;
                itemsList[i].gameObject.SetActive(isCurrentTask);
            }
        }
        
        // Activar/desactivar slots según la tarea actual
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] != null)
            {
                bool isCurrentTask = slotTaskIndices[i] == currentTaskIndex;
                itemSlots[i].gameObject.SetActive(isCurrentTask);
            }
        }
    }

    private int CalculateCorrectPlacementsForCurrentTask()
    {
        int count = 0;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (slotTaskIndices[i] == currentTaskIndex)
            {
                int slotPos = itemSlots[i].slotPosition;
                if (slotCompletionStatus.ContainsKey(slotPos) && slotCompletionStatus[slotPos])
                {
                    count++;
                }
            }
        }
        return count;
    }

    public void OnFragmentCorrectlyPlaced(int slotPosition)
    {
        if (isMinigameComplete) return;
        Debug.Log("OnFragmentCorrectlyPlaced");
        slotCompletionStatus[slotPosition] = true;
        correctPlacements++;

        Debug.Log($"Total items colocados correctamente: {correctPlacements}");
        LightManager.Instance.UpdateLighting();
        onFragmentPlaced?.Invoke(slotPosition);
        CheckTaskCompletion();
    }

    private void CheckTaskCompletion()
{
    Debug.Log($"Verificando tarea {currentTaskIndex}...");
    
    // Verificar si todos los slots de la tarea actual están completos
    bool taskComplete = true;
    
    // Recorrer todos los slots
    for (int i = 0; i < itemSlots.Length; i++)
    {
        // Verificar solo los slots de la tarea actual
        if (slotTaskIndices[i] == currentTaskIndex)
        {
            int slotPosition = itemSlots[i].slotPosition;
            bool slotComplete = slotCompletionStatus.ContainsKey(slotPosition) && slotCompletionStatus[slotPosition];
            Debug.Log($"Slot {slotPosition} (tarea {currentTaskIndex}), completado: {slotComplete}");
            
            if (!slotComplete)
            {
                taskComplete = false;
                break;
            }
        }
    }
    
    Debug.Log($"Tarea {currentTaskIndex} completa: {taskComplete}");
    
    if (taskComplete)
    {
        Debug.Log($"¡Tarea {currentTaskIndex} completada!");
        onTaskComplete?.Invoke();
        
        // Verificar si es la última tarea
        if (currentTaskIndex >= taskCameras.Length - 1)
        {
            Debug.Log("Todas las tareas completadas, finalizando minijuego");
            // Añadir un pequeño retraso antes de completar el minijuego
            StartCoroutine(DelayedCompleteMinigame());
        }
        else
        {
            Debug.Log("Iniciando transición a la siguiente tarea");
            
            // Activar el efecto de fade out
            if (fadeOutEffectChangeCamera != null)
            {
                fadeOutEffectChangeCamera.SetActive(true);
                
                // Asegurarse de que el efecto de fade se inicie
                GDTFadeEffect fadeEffect = fadeOutEffectChangeCamera.GetComponent<GDTFadeEffect>();
                if (fadeEffect != null)
                {
                    fadeEffect.StartEffect();
                    Debug.Log("Efecto de fade iniciado manualmente");
                }
                
                StartCoroutine(TransitionToNextTask());
            }
        }
    }
}

    // Añadir este nuevo método para retrasar ligeramente la finalización
    private IEnumerator DelayedCompleteMinigame()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.Play("PuzleSolvedColorsin");
        }
        // Esperar un momento para que el jugador vea que colocó correctamente el último objeto
        yield return new WaitForSeconds(1.5f);

        // Activar efecto de fade out final si existe
        if (fadeOutEffect != null)
        {
            fadeOutEffect.SetActive(true);

            // Asegurarse de que el efecto de fade se inicie
            GDTFadeEffect fadeEffect = fadeOutEffect.GetComponent<GDTFadeEffect>();
            if (fadeEffect != null)
            {
                fadeEffect.StartEffect();
            }
            // Esperar a que termine el fade
            yield return new WaitForSeconds(1f);
        }

        // Completar el minijuego
        CompleteMinigame();
    }

    private IEnumerator TransitionToNextTask()
    {
        Debug.Log("Iniciando coroutine TransitionToNextTask");

        // Esperar 1 segundo para el fade out
        yield return new WaitForSeconds(1f);
        
        // Cambiar a la siguiente tarea
        currentTaskIndex++;
        Debug.Log($"Cambiando a tarea {currentTaskIndex}");
        
        SwitchToCamera(currentTaskIndex);
        
        // Actualizar items y slots visibles
        UpdateActiveItemsAndSlots();
        
        // Desactivar el efecto de fade out
        if (fadeOutEffectChangeCamera != null)
        {
            fadeOutEffectChangeCamera.SetActive(false);
            Debug.Log("Efecto de fade desactivado");
        }

        if (currentTaskIndex == 1)
        {
            mc.DisplayComment(1);
        }
        if (currentTaskIndex == 2)
        {
            mc.DisplayComment(2);
        }
        if (currentTaskIndex == 3)
        {
            mc.DisplayComment(3);
        }
    }

    private void SwitchToCamera(int index)
    {
        for (int i = 0; i < taskCameras.Length; i++)
        {
            if (taskCameras[i] != null)
            {
                taskCameras[i].gameObject.SetActive(i == index);
            }
        }
    }

    public void CompleteMinigame()
    {
        Debug.Log("Ejecutando CompleteMinigame()");
    
        // Evitar llamadas múltiples
        if (isMinigameComplete)
        {
            Debug.Log("El minijuego ya está marcado como completado. Ignorando llamada.");
            return;
        }
    
        // Marcar como completado
        isMinigameComplete = true;
        Debug.Log("Minijuego marcado como completado");
    
        // Invocar evento de finalización
        onMinigameComplete?.Invoke();
        Debug.Log("Evento onMinigameComplete invocado");
    
        // Llamar al GameManager para volver al lobby
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.MinigameCompleted(0);
            Debug.Log("Llamando a GameManager.MinigameCompleted(0)");
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }
    }

    public void ResetMinigame()
    {
        correctPlacements = 0;
        isMinigameComplete = false;
        foreach (var key in slotCompletionStatus.Keys.ToList())
        {
            slotCompletionStatus[key] = false;
        }
        foreach (var fragment in itemsList)
        {
            fragment.ResetPosition();
        }

        currentTaskIndex = 0;
        SwitchToCamera(currentTaskIndex);
        UpdateActiveItemsAndSlots();
    }

    public int GetTotalFragments() => itemSlots.Length;
    public bool IsSlotCompleted(int slotPosition) => 
        slotCompletionStatus.ContainsKey(slotPosition) && slotCompletionStatus[slotPosition];
    public bool IsMinigameComplete() => isMinigameComplete;

    private void OnDestroy() => Instance = null;
    public int GetCorrectPlacements() => correctPlacements;
}