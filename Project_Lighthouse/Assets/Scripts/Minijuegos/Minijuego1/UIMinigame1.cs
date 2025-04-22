using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIMinigame1 : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject taskFeedbackPanel;

    [Header("Progreso")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI taskHeader;
    
    [Header("Instrucciones")]
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private string[] taskInstructions; // Instrucciones para cada tarea
    
    [Header("Feedback")]
    [SerializeField] private GameObject correctFeedback;
    [SerializeField] private GameObject incorrectFeedback;
    [SerializeField] private TextMeshProUGUI taskFeedbackText; // Referencia al texto del panel de feedback
    
    [Header("Finalización")]
    [SerializeField] private TextMeshProUGUI completionTitle;
    [SerializeField] private TextMeshProUGUI completionMessage;
    
    [Header("Referencias")]
    [SerializeField] private LevelGameManagerMinigame1 gameManager;

    private int lastTaskIndex = -1;
    private bool hasStarted = false;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<LevelGameManagerMinigame1>();
            
        // Registrar eventos
        gameManager.onMinigameStart.AddListener(HandleMinigameStart);
        gameManager.onMinigameComplete.AddListener(HandleMinigameComplete);
        gameManager.onFragmentPlaced.AddListener(HandleFragmentPlaced);
        gameManager.onTaskComplete.AddListener(HandleTaskComplete);
        
        // Configuración inicial
        if (startPanel != null) startPanel.SetActive(true);
        if (completionPanel != null) completionPanel.SetActive(false);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (taskFeedbackPanel != null) taskFeedbackPanel.SetActive(false);
        if (correctFeedback != null) correctFeedback.SetActive(false);
        if (incorrectFeedback != null) incorrectFeedback.SetActive(false);
        
        UpdateUI();
        
        // Para pruebas - mostrar panel de instrucciones al inicio
        ShowInitialPanels();
    }
    
    // Nueva función para mostrar paneles iniciales
    private void ShowInitialPanels()
    {
        // Mostrar instrucciones iniciales
        if (instructionsPanel != null && taskInstructions != null && taskInstructions.Length > 0)
        {
            Debug.Log("Mostrando panel de instrucciones inicial");
            instructionsText.text = "¡Comienza a ordenar los objetos en sus lugares correspondientes!";
            instructionsPanel.SetActive(true);
            
            // Ocultarlo después de un tiempo
            StartCoroutine(HideInstructionsAfterDelay(4f));
        }
        
        // Mostrar panel de feedback inicial
        if (taskFeedbackPanel != null)
        {
            Debug.Log("Mostrando panel de feedback inicial");
            
            if (taskFeedbackText != null)
                taskFeedbackText.text = "¡Tarea 1: Ordena los objetos!";
                
            taskFeedbackPanel.SetActive(true);
            
            // Ocultarlo después de un tiempo
            StartCoroutine(HideFeedbackPanelAfterDelay(3f));
        }
    }
    
    private IEnumerator HideFeedbackPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (taskFeedbackPanel != null)
            taskFeedbackPanel.SetActive(false);
    }
    
    public void StartMinigame()
    {
        hasStarted = true;
        
        if (gameManager != null)
        {
            if (startPanel != null) startPanel.SetActive(false);
            
            // Mostrar panel de feedback antes de iniciar
            ShowTaskFeedback("¡Comenzando tarea 1!");
            
            // Esperar un momento antes de iniciar el minijuego
            StartCoroutine(DelayedStartMinigame());
        }
    }
    
    private IEnumerator DelayedStartMinigame()
    {
        yield return new WaitForSeconds(1f);
        gameManager.StartMinigame();
        
        // Mostrar instrucciones después de iniciar
        yield return new WaitForSeconds(0.5f);
        ShowTaskInstructions(gameManager.currentTaskIndex);
    }
    
    private void HandleMinigameStart()
    {
        Debug.Log("Evento onMinigameStart recibido");
        UpdateUI();
        
        // Mostrar feedback e instrucciones
        ShowTaskFeedback("¡Minijuego iniciado!");
        ShowTaskInstructions(gameManager.currentTaskIndex);
    }
    
    private void HandleMinigameComplete()
    {
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
            
            if (completionTitle != null)
                completionTitle.text = "¡Minijuego Completado!";
                
            if (completionMessage != null)
                completionMessage.text = $"Has completado todas las tareas con éxito.\n" +
                                        $"Objetos colocados: {gameManager.GetCorrectPlacements()} / {gameManager.GetTotalFragments()}";
        }
    }
    
    private void HandleFragmentPlaced(int slotPosition)
    {
        // Actualizar barra de progreso
        UpdateUI();
        
        // Mostrar feedback visual
        ShowFeedback(true);
    }
    
    public void HandleIncorrectPlacement()
    {
        // Mostrar feedback negativo
        ShowFeedback(false);
    }
    
    private void HandleTaskComplete()
    {
        Debug.Log("Tarea completada, mostrando feedback");
        
        // Obtener el nombre de la próxima tarea
        int nextTaskIndex = gameManager.currentTaskIndex + 1;
        string taskName = "final";
        
        if (nextTaskIndex < 3) // Ajusta este número según la cantidad de tareas
        {
            string[] taskNames = new string[] { "Cajones", "Estantería", "Cubiertos" };
            if (nextTaskIndex < taskNames.Length)
                taskName = taskNames[nextTaskIndex];
            else
                taskName = $"Tarea {nextTaskIndex + 1}";
        }
        
        // Mostrar mensaje de tarea completada
        ShowTaskFeedback($"¡Tarea completada! Pasando a {taskName}");
    }
    
    // Nuevo método para mostrar feedback de tarea con texto personalizado
    private void ShowTaskFeedback(string message)
    {
        if (taskFeedbackPanel != null)
        {
            if (taskFeedbackText != null)
                taskFeedbackText.text = message;
                
            taskFeedbackPanel.SetActive(true);
            
            // Ocultar después de un tiempo
            StartCoroutine(HideFeedbackPanelAfterDelay(2.5f));
        }
    }
    
    private void ShowTaskInstructions(int taskIndex)
    {
        Debug.Log($"ShowTaskInstructions llamado para tarea {taskIndex}");
        
        // Actualizar el último índice conocido
        lastTaskIndex = taskIndex;
        
        if (instructionsPanel != null && instructionsText != null && taskInstructions != null)
        {
            if (taskIndex >= 0 && taskIndex < taskInstructions.Length)
            {
                instructionsText.text = taskInstructions[taskIndex];
                instructionsPanel.SetActive(true);
                
                Debug.Log($"Mostrando instrucciones: '{taskInstructions[taskIndex]}'");
                
                // Ocultar tras unos segundos
                StopAllCoroutines(); // Detener cualquier coroutine anterior de ocultar
                StartCoroutine(HideInstructionsAfterDelay(4f));
            }
            else
            {
                Debug.LogWarning($"Índice de tarea fuera de rango: {taskIndex}, máximo: {taskInstructions.Length-1}");
            }
        }
        else
        {
            Debug.LogWarning("No se pueden mostrar instrucciones, falta algún componente");
        }
    }
    
    private IEnumerator HideInstructionsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (instructionsPanel != null)
        {
            Debug.Log("Ocultando panel de instrucciones");
            instructionsPanel.SetActive(false);
        }
    }
    
    private void ShowFeedback(bool correct)
    {
        if (correct && correctFeedback != null)
        {
            correctFeedback.SetActive(true);
            StartCoroutine(HideFeedback(correctFeedback, 1f));
        }
        else if (!correct && incorrectFeedback != null)
        {
            incorrectFeedback.SetActive(true);
            StartCoroutine(HideFeedback(incorrectFeedback, 1f));
        }
    }
    
    private IEnumerator HideFeedback(GameObject feedback, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedback != null)
            feedback.SetActive(false);
    }
    
    public void UpdateUI()
    {
        if (gameManager != null)
        {
            int total = gameManager.GetTotalFragments();
            int correct = gameManager.GetCorrectPlacements();
            
            // Actualizar texto y barra de progreso
            if (progressText != null)
                progressText.text = $"{correct}/{total}";
                
            if (progressBar != null)
                progressBar.value = total > 0 ? (float)correct / total : 0;
                
            // Actualizar título de la tarea
            if (taskHeader != null)
            {
                //string[] taskNames = new string[] { "Cajones", "Estantería", "Cubiertos" }; Esto se utilizara para las siguientes entregas
                string[] taskNames = new string[] { "Cubiertos", "Cubiertos_2", "Cubiertos_3" }; // Personalizar según tus tareas
                int taskIndex = 0;
                
                if (taskIndex >= 0 && taskIndex < taskNames.Length)
                    taskHeader.text = taskNames[taskIndex];
                else
                    taskHeader.text = $"Tarea {taskIndex}";
                taskIndex++;
            }
        }
    }
    
    public void ShowHelpPanel()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }
    
    public void HideHelpPanel()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }
    
    public void RestartMinigame()
    {
        if (gameManager != null)
        {
            gameManager.ResetMinigame();
            completionPanel.SetActive(false);
            
            // Mostrar paneles iniciales de nuevo
            ShowInitialPanels();
        }
    }
}