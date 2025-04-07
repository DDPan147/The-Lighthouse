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

    [Header("Game State")]
    [SerializeField] private bool gameActive = false;
    [SerializeField] private int totalRepairTasks = 3; // Clock, Table, Cleaning
    [SerializeField] private int completedTasks = 0;

    [Header("Player References")]
    public PlayerMovementFP playerMovement;
    public CameraController playerCamera;

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

    private IEnumerator CompleteLevel()
    {
        yield return new WaitForSeconds(1f);

        playerMovement.canMove = false;

        // Show completion UI
        if (completionPanel != null)
            completionPanel.SetActive(true);
    }

    // Methods to connect with your existing scripts
    public void OnClockFixed()
    {
        RegisterTaskCompletion("Clock");
    }

    public void OnTableFixed()
    {
        RegisterTaskCompletion("Table");
    }

    public void OnCleaningComplete()
    {
        RegisterTaskCompletion("Cleaning");
    }
}
