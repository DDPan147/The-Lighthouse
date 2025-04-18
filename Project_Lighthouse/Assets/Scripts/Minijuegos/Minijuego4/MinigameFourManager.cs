using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinigameFourManager : MonoBehaviour
{
    #region Singleton
    public static MinigameFourManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion
    
    public enum RepairStage { CollectingItems, RepairingTable, RepairingClock, RepairingDoll, Completed }
    public RepairStage currentStage = RepairStage.CollectingItems;
    
    [Header("Collected Items")]
    public int tableLegsCollected = 0;
    public int clockGearsCollected = 0;
    public int dollPartsCollected = 0;
    public GameObject[] tableLegVisuals;
    public GameObject[] clockGearVisuals;
    public GameObject[] dollPartVisuals;
    
    [Header("Required Items")]
    public int requiredTableLegs = 4;
    public int requiredClockGears = 5;
    public int requiredDollParts = 3;
    
    [Header("Repair Stations")]
    public GameObject tableRepairStation;
    public GameObject clockRepairStation;
    public GameObject dollRepairStation;
    
    [Header("UI References")]
    public TextMeshProUGUI inventoryText; // Para mostrar el inventario actual

    [Header("Game State")]
    [SerializeField] private bool gameActive = false;
    [SerializeField] private int totalRepairTasks = 3; // Clock, Table, Cleaning
    [SerializeField] private int completedTasks = 0;
    
    [Header("Player References")]
    public PlayerMovementFP playerMovement;
    public CameraController playerCamera;
    
    [Header("Inventory Integration")]
    public PlayerInventory playerInventory;

    [Header("Repair Objects")]
    public ClockRepairMode clockRepair;
    public MesaReparacion tableRepair;
    public CleanableTexture[] cleanableObjects;

    [Header("UI Elements")]
    public TMP_Text objectiveText;
    public TMP_Text progressText;
    public GameObject completionPanel;
    
    private Dictionary<string, bool> repairStatus = new Dictionary<string, bool>();
    
    private void Start()
    {
        if (tableRepairStation) tableRepairStation.SetActive(false);
        if (clockRepairStation) clockRepairStation.SetActive(false);
        if (dollRepairStation) dollRepairStation.SetActive(false);
        
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();
        DisableAllVisualObjects();
        
        UpdateInventoryUI();
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Find references if not assigned
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovementFP>();

        if (playerCamera == null)
            playerCamera = FindObjectOfType<CameraController>();

        // Initialize repair status tracking
        repairStatus.Add("Table", false);
        repairStatus.Add("Clock", false);
        repairStatus.Add("Doll", false);
        totalRepairTasks = 3; // Table, Clock, Doll

        // Setup UI
        UpdateUI();

        // Start game
        gameActive = true;
    }

    public void RegisterTaskCompletion(string taskName)
    {
        if (repairStatus.ContainsKey(taskName) && !repairStatus[taskName])
        {
            repairStatus[taskName] = true;
            completedTasks++;
            UpdateUI();
            CheckLevelCompletion();
        }
    }
    public void AddItemToInventory(ItemType type)
    {
        switch(type) {
            case ItemType.TableLeg:
                tableLegsCollected++;
                Debug.Log($"[MinigameFourManager] Pata de mesa recogida. Total: {tableLegsCollected}");
                if (tableLegsCollected-1 < tableLegVisuals.Length)
                    tableLegVisuals[tableLegsCollected-1].SetActive(true);
                break;
            case ItemType.ClockGear:
                clockGearsCollected++;
                Debug.Log($"[MinigameFourManager] Engranaje recogido. Total: {clockGearsCollected}");
                if (clockGearsCollected-1 < clockGearVisuals.Length)
                    clockGearVisuals[clockGearsCollected-1].SetActive(true);
                break;
            case ItemType.DollPart:
                dollPartsCollected++;
                Debug.Log($"[MinigameFourManager] Pieza de muñeco recogida. Total: {dollPartsCollected}");
                if (dollPartsCollected-1 < dollPartVisuals.Length)
                    dollPartVisuals[dollPartsCollected-1].SetActive(true);
                break;
        }
    
        CheckCollectionProgress();
    }
    
    private void UpdateInventoryUI()
    {
        if (inventoryText != null)
        {
            inventoryText.text = $"Patas de mesa: {tableLegsCollected}/{requiredTableLegs}\n" +
                                 $"Engranajes: {clockGearsCollected}/{requiredClockGears}\n" +
                                 $"Piezas de muñeco: {dollPartsCollected}/{requiredDollParts}";
        }
    }
    
    private void CheckCollectionProgress()
    {
        if (tableLegsCollected >= requiredTableLegs && 
            clockGearsCollected >= requiredClockGears && 
            dollPartsCollected >= requiredDollParts)
        {
            // Si estamos en la fase de recolección, avanzar a la siguiente fase
            if (currentStage == RepairStage.CollectingItems)
            {
                AdvanceToNextStage();
            }
        }
    }
    public void AdvanceToNextStage()
    {
        switch(currentStage) {
            case RepairStage.CollectingItems:
                currentStage = RepairStage.RepairingTable;
                EnableTableRepair();
                break;
            case RepairStage.RepairingTable:
                currentStage = RepairStage.RepairingClock;
                EnableClockRepair();
                break;
            case RepairStage.RepairingClock:
                currentStage = RepairStage.RepairingDoll;
                EnableDollRepair();
                break;
            case RepairStage.RepairingDoll:
                currentStage = RepairStage.Completed;
                CheckLevelCompletion();
                break;
        }
    }
    
    // Métodos para activar cada estación de reparación
    private void EnableTableRepair()
    {
        Debug.Log("Activando reparación de mesa");
        if (tableRepairStation) tableRepairStation.SetActive(true);
    }
    
    private void EnableClockRepair()
    {
        Debug.Log("Activando reparación de reloj");
        if (tableRepairStation) tableRepairStation.SetActive(false);
        if (clockRepairStation) clockRepairStation.SetActive(true);
    }
    
    private void EnableDollRepair()
    {
        Debug.Log("Activando reparación de muñeco");
        if (clockRepairStation) clockRepairStation.SetActive(false);
        if (dollRepairStation) dollRepairStation.SetActive(true);
    }
    
    // Callbacks desde los minijuegos
    public void OnTableFixed()
    {
        Debug.Log("Mesa reparada completamente");
        
        RegisterTaskCompletion("Table");
        AdvanceToNextStage();
        for (int i = 0; i < tableLegVisuals.Length; i++)
        {
            tableLegVisuals[i].SetActive(false);
        }
        // Añadir un delay corto y luego activar automáticamente el modo reparación del reloj
        StartCoroutine(ActivateClockRepairAfterDelay(.1f));
    }
    private IEnumerator ActivateClockRepairAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    
        // Buscar el componente ClockRepairMode y activarlo
        ClockRepairMode clockRepair = null;
        if (clockRepairStation != null)
        {
            clockRepair = clockRepairStation.GetComponent<ClockRepairMode>();
        }
    
        if (clockRepair == null)
        {
            clockRepair = FindObjectOfType<ClockRepairMode>();
        }
    
        if (clockRepair != null)
        {
            clockRepair.AutoEnterRepairMode();
        }
        else
        {
            Debug.LogError("No se encontró el componente ClockRepairMode");
        }
    }
    
    public void OnClockFixed()
    {
        
        RegisterTaskCompletion("Clock");
        Debug.Log("Reloj reparado completamente");
        for (int i = 0; i < clockGearVisuals.Length; i++)
        {
            clockGearVisuals[i].SetActive(false);
        }
        AdvanceToNextStage();
        StartCoroutine(ActivateDollRepairAfterDelay(.1f));
    }
    
    private IEnumerator ActivateDollRepairAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    
        // Buscar el componente DollRepair y activarlo
        DollRepair dollRepair = null;
        if (dollRepairStation != null)
        {
            dollRepair = dollRepairStation.GetComponent<DollRepair>();
        }
    
        if (dollRepair == null)
        {
            dollRepair = FindObjectOfType<DollRepair>();
        }
    
        if (dollRepair != null)
        {
            dollRepair.AutoEnterRepairMode();
        }
        else
        {
            Debug.LogError("No se encontró el componente DollRepair");
        }
    }
    
    public void OnDollFixed()
    {
        Debug.Log("Muñeco reparado completamente");
        RegisterTaskCompletion("Doll"); // Añadir esta línea para registrar la tarea
    
        // Ocultar visuales de piezas de muñeco
        for (int i = 0; i < dollPartVisuals.Length; i++)
        {
            dollPartVisuals[i].SetActive(false);
        }
    
        AdvanceToNextStage();
    
        // Verificar explícitamente si se han completado todas las tareas
        CheckLevelCompletion();
    }

    private void UpdateUI()
    {
        if (progressText != null)
            progressText.text = $"Progress: {completedTasks}/{totalRepairTasks}";
    
        // Actualizar el texto de objetivo según la etapa actual
        if (objectiveText != null)
        {
            switch(currentStage)
            {
                case RepairStage.CollectingItems:
                    objectiveText.text = "Recolecta todas las piezas necesarias";
                    break;
                case RepairStage.RepairingTable:
                    objectiveText.text = "Repara la mesa";
                    break;
                case RepairStage.RepairingClock:
                    objectiveText.text = "Repara el reloj";
                    break;
                case RepairStage.RepairingDoll:
                    objectiveText.text = "Repara la muñeca";
                    break;
                case RepairStage.Completed:
                    objectiveText.text = "¡Minijuego completado!";
                    break;
            }
        }
    }

    private void CheckLevelCompletion()
    {
        Debug.Log($"Verificando completado del nivel: {completedTasks}/{totalRepairTasks} tareas completadas");
    
        // Imprimir el estado de cada tarea para depuración
        foreach (var task in repairStatus)
        {
            Debug.Log($"Tarea '{task.Key}': {(task.Value ? "Completada" : "Pendiente")}");
        }
    
        if (completedTasks >= totalRepairTasks || currentStage == RepairStage.Completed)
        {
            Debug.Log("¡Nivel completado! Iniciando secuencia de finalización...");
            StartCoroutine(CompleteLevel());
        }
    }
    
    // Método para comprobar si tenemos suficientes piezas para una reparación específica
    public bool HasEnoughParts(ItemType type, int requiredAmount)
    {
        // if (playerInventory != null)
        // {
        //     return playerInventory.HasEnoughItems(type, requiredAmount);
        // }
        // return false;
        
        switch(type) {
            case ItemType.TableLeg:
                return tableLegsCollected >= requiredAmount;
            case ItemType.ClockGear:
                return clockGearsCollected >= requiredAmount;
            case ItemType.DollPart:
                return dollPartsCollected >= requiredAmount;
            default:
                return false;
        }
    }

    private IEnumerator CompleteLevel()
    {
        yield return new WaitForSeconds(.3f);

        playerMovement.canMove = false;

        // Show completion UI
        if (completionPanel != null)
            completionPanel.SetActive(true);

        /*Alvaro*/ //Function to complete minigame and return to lobby
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.MinigameCompleted(3);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }
    }

    public void DebugCompleteLevel()
    {
        // Show completion UI
        if (completionPanel != null)
            completionPanel.SetActive(true);

        /*Alvaro*/ //Function to complete minigame and return to lobby
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.MinigameCompleted(3);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }
    }
    
    private void DisableAllVisualObjects()
    {
        // Desactivar visuales de patas de mesa
        if (tableLegVisuals != null)
        {
            foreach (GameObject visual in tableLegVisuals)
            {
                if (visual != null)
                {
                    visual.SetActive(false);
                }
            }
        }
    
        // Desactivar visuales de engranajes
        if (clockGearVisuals != null)
        {
            foreach (GameObject visual in clockGearVisuals)
            {
                if (visual != null)
                {
                    visual.SetActive(false);
                }
            }
        }
    
        // Desactivar visuales de piezas de muñeco
        if (dollPartVisuals != null)
        {
            foreach (GameObject visual in dollPartVisuals)
            {
                if (visual != null)
                {
                    visual.SetActive(false);
                }
            }
        }
    }

    public void OnCleaningComplete()
    {
        RegisterTaskCompletion("Cleaning");
    }
}
