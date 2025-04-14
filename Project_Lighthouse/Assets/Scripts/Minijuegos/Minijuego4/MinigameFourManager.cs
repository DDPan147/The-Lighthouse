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
        repairStatus.Add("Clock", false);
        repairStatus.Add("Table", false);
        repairStatus.Add("Cleaning", false);

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
                break;
            case ItemType.ClockGear:
                clockGearsCollected++;
                break;
            case ItemType.DollPart:
                dollPartsCollected++;
                break;
        }
        
        UpdateInventoryUI();
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
    }
    
    public void OnClockFixed()
    {
        
        RegisterTaskCompletion("Clock");
        Debug.Log("Reloj reparado completamente");
        AdvanceToNextStage();
    }
    
    public void OnDollFixed()
    {
        Debug.Log("Muñeco reparado completamente");
        AdvanceToNextStage();
    }

    private void UpdateUI()
    {
        if (progressText != null)
            progressText.text = $"Progress: {completedTasks}/{totalRepairTasks}";
    }

    private void CheckLevelCompletion()
    {
        if (completedTasks >= totalRepairTasks)
        {
            StartCoroutine(CompleteLevel());
        }
    }
    
    // Método para comprobar si tenemos suficientes piezas para una reparación específica
    public bool HasEnoughParts(ItemType type, int requiredAmount)
    {
        if (playerInventory != null)
        {
            return playerInventory.HasEnoughItems(type, requiredAmount);
        }
        return false;
        
        // switch(type) {
        //     case ItemType.TableLeg:
        //         return tableLegsCollected >= requiredAmount;
        //     case ItemType.ClockGear:
        //         return clockGearsCollected >= requiredAmount;
        //     case ItemType.DollPart:
        //         return dollPartsCollected >= requiredAmount;
        //     default:
        //         return false;
        // }
    }

    private IEnumerator CompleteLevel()
    {
        yield return new WaitForSeconds(1f);

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

    public void OnCleaningComplete()
    {
        RegisterTaskCompletion("Cleaning");
    }
}
