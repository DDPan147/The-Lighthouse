using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class WindowRestorationManager : MonoBehaviour
{
    public static WindowRestorationManager Instance { get; private set; }

    [Header("Referencias de la Escena")]
    [SerializeField] private DragDrop_Item[] glassFragments;
    [SerializeField] private DragDrop_Slot[] windowSlots;
    
    [Header("Estado del Juego")]
    [SerializeField] private int correctPlacements = 0;
    private bool isMinigameComplete = false;
    private Dictionary<int, bool> slotCompletionStatus;
    
    [Header("Eventos")]
    public UnityEvent onMinigameStart;
    public UnityEvent onMinigameComplete;
    public UnityEvent<int> onFragmentPlaced;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        InitializeMinigame();
    }

    private void InitializeMinigame()
    {
        slotCompletionStatus = new Dictionary<int, bool>();
        foreach (var slot in windowSlots)
        {
            slotCompletionStatus[slot.slotPosition] = false;
        }
        ValidateSetup();
    }

    private void ValidateSetup()
    {
        if (glassFragments == null || glassFragments.Length == 0)
        {
            Debug.LogError("WindowRestorationManager: No glass fragments assigned!");
            return;
        }
        if (windowSlots == null || windowSlots.Length == 0)
        {
            Debug.LogError("WindowRestorationManager: No window slots assigned!");
            return;
        }
        if (glassFragments.Length != windowSlots.Length)
        {
            Debug.LogWarning("WindowRestorationManager: Mismatch between fragments and slots count!");
        }
    }

    public void StartMinigame()
    {
        correctPlacements = 0;
        isMinigameComplete = false;
        foreach (var key in slotCompletionStatus.Keys.ToList())
        {
            slotCompletionStatus[key] = false;
        }
        onMinigameStart?.Invoke();
    }

    public void OnFragmentCorrectlyPlaced(int slotPosition)
    {
    
        if (isMinigameComplete) return;
        slotCompletionStatus[slotPosition] = true;
        correctPlacements = slotCompletionStatus.Count(kvp => kvp.Value);
        onFragmentPlaced?.Invoke(slotPosition);
        CheckMinigameCompletion();
        Debug.Log($"Fragment placed in slot {slotPosition}. Total correct: {correctPlacements}");
    }

    private void CheckMinigameCompletion()
    {
        if (correctPlacements >= windowSlots.Length)
        {
            CompleteMinigame();
        }
    }

    private void CompleteMinigame()
    {
        if (isMinigameComplete) return;
        isMinigameComplete = true;
        onMinigameComplete?.Invoke();
    }

    public void ResetMinigame()
    {
        correctPlacements = 0;
        isMinigameComplete = false;
        foreach (var key in slotCompletionStatus.Keys.ToList())
        {
            slotCompletionStatus[key] = false;
        }
        foreach (var fragment in glassFragments)
        {
            fragment.ResetPosition();
        }
    }

    public int GetTotalFragments() => windowSlots.Length;
    public bool IsSlotCompleted(int slotPosition) => 
        slotCompletionStatus.ContainsKey(slotPosition) && slotCompletionStatus[slotPosition];
    public bool IsMinigameComplete() => isMinigameComplete;

    private void OnDestroy() => Instance = null;
}