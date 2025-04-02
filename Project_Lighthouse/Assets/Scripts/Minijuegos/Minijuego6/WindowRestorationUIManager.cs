using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowRestorationUIManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject completionPanel;
    
    [Header("Progreso")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressBar;
    
    [Header("Referencias")]
    [SerializeField] private WindowRestorationManager gameManager;

    
    private void Start()
    {
        SetupGameManager();
        SetupUI();
        SubscribeToEvents();
    }
    
    private void SetupGameManager()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<WindowRestorationManager>();
            if (gameManager == null)
            {
                Debug.LogError("No se encontró WindowRestorationManager en la escena!");
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
    }

    private void SubscribeToEvents()
    {
        // Limpiar eventos previos si existieran
        gameManager.onMinigameStart.RemoveAllListeners();
        gameManager.onFragmentPlaced.RemoveAllListeners();
        gameManager.onMinigameComplete.RemoveAllListeners();

        gameManager.onMinigameStart.AddListener(() => {
            Debug.Log("Minigame Started");
            UpdateProgress(0);
        });

        gameManager.onFragmentPlaced.AddListener((int slotPosition) => {
            Debug.Log($"UI Update - Fragment Placed in slot {slotPosition}");
            UpdateProgress(gameManager.GetCorrectPlacements()); // Añadir este método al manager
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
            gameManager.onMinigameComplete.RemoveAllListeners();
        }
    }

    public void OnContinueButtonClicked()
    {
        // Aquí puedes poner la lógica para continuar al siguiente nivel
        // o cerrar el minijuego
    }
    
}