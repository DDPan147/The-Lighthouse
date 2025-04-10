using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMinigame1 : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject completionPanel;
    
    [Header("Progreso")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI taskText; // Texto para mostrar la tarea actual
    
    [Header("Referencias")]
    [SerializeField] private LevelGameManagerMinigame1 gameManager;

    // Nombres de las tareas para mostrar en la UI
    private string[] taskNames = { "Cajones", "Estanterías", "Bandeja de Cubiertos" };
    
    private void Start()
    {
        SetupGameManager();
        SetupUI();
        SubscribeToEvents();
        gameManager.onTaskComplete.AddListener(() => {
            Debug.Log("Evento onTaskComplete disparado");
        });
    }
    
    private void SetupGameManager()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<LevelGameManagerMinigame1>();
            if (gameManager == null)
            {
                Debug.LogError("No se encontró LevelGameManagerMinigame1 en la escena!");
                return;
            }
        }
    }

    private void SetupUI()
    {
        startPanel.SetActive(true);
        completionPanel.SetActive(false);
        progressBar.maxValue = gameManager.GetTotalFragments();
        UpdateProgress(0);
        
        if (taskText != null && taskNames.Length > 0)
        {
            taskText.text = $"Tarea: {taskNames[0]}";
        }
    }

    private void SubscribeToEvents()
    {
        // Limpiar eventos previos si existieran
        gameManager.onMinigameStart.RemoveAllListeners();
        gameManager.onFragmentPlaced.RemoveAllListeners();
        gameManager.onTaskComplete.RemoveAllListeners();
        gameManager.onMinigameComplete.RemoveAllListeners();

        gameManager.onMinigameStart.AddListener(() => {
            Debug.Log("Minigame Started");
            UpdateProgress(0);
            if (taskText != null && taskNames.Length > 0)
            {
                taskText.text = $"Tarea: {taskNames[0]}";
            }
        });

        gameManager.onFragmentPlaced.AddListener((int slotPosition) => {
            Debug.Log($"UI Update - Fragment Placed in slot {slotPosition}");
            UpdateProgress(gameManager.GetCorrectPlacements());
        });
        
        gameManager.onTaskComplete.AddListener(() => {
            Debug.Log("Task Completed");
            // Actualizar el texto de la tarea si hay una siguiente tarea
            if (taskText != null)
            {
                int nextTaskIndex = gameManager.currentTaskIndex + 1;
                if (nextTaskIndex < taskNames.Length)
                {
                    taskText.text = $"Tarea: {taskNames[nextTaskIndex]}";
                }
            }
        });

        gameManager.onMinigameComplete.AddListener(() => {
            Debug.Log("Minigame Completed");
            ShowCompletionScreen();
        });
    }

    public void OnStartButtonClicked()
    {
        startPanel.SetActive(false);
        gameManager.StartMinigame();
    }

    public void UpdateProgress(int fragmentsPlaced)
    {
        if (progressText != null)
        {
            progressText.text = $"Fragmentos: {fragmentsPlaced}/{gameManager.GetTotalFragments()}";
        }
        if (progressBar != null)
        {
            progressBar.value = fragmentsPlaced;
        }
    }

    public void ShowCompletionScreen()
    {
        completionPanel.SetActive(true);
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            FindFirstObjectByType<GameManager>().MinigameCompleted(0);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }
    }
    
    private void OnDestroy()
    {
        // Desuscribirse de los eventos para evitar referencias nulas
        if (gameManager != null)
        {
            gameManager.onMinigameStart.RemoveAllListeners();
            gameManager.onFragmentPlaced.RemoveAllListeners();
            gameManager.onTaskComplete.RemoveAllListeners();
            gameManager.onMinigameComplete.RemoveAllListeners();
        }
    }

    public void OnContinueButtonClicked()
    {
        // Aquí puedes poner la lógica para continuar al siguiente nivel
        // o cerrar el minijuego
    }
}