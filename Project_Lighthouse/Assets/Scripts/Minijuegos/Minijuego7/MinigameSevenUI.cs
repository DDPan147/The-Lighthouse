using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinigameSevenUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MinigameSevenManager gameManager;
    
    [Header("Panel de Progreso")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;
    
    [Header("Estado Emocional")]
    [SerializeField] private Slider emotionalStateSlider;
    
    [Header("Información de Objeto")]
    [SerializeField] private GameObject objectInfoPanel;
    [SerializeField] private Image objectIcon;
    [SerializeField] private TMP_Text objectNameText;
    [SerializeField] private TMP_Text objectDescriptionText;
    [SerializeField] private TMP_Text emotionalValueText;
    
    [Header("Contadores")]
    [SerializeField] private TMP_Text savedObjectsText;
    [SerializeField] private TMP_Text discardedObjectsText;
    
    [Header("Prompts")]
    [SerializeField] private GameObject promptsPanel;
    [SerializeField] private TMP_Text actionPromptText;
    
    [Header("Animaciones")]
    [SerializeField] private Animator uiAnimator;
    
    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<MinigameSevenManager>();
            
        // Inicializar UI
        UpdateProgressBar();
        UpdateEmotionalState();
        UpdateObjectCounters();
        
        // Ocultar panel de información de objeto inicialmente
        if (objectInfoPanel != null)
            objectInfoPanel.SetActive(false);
    }
    
    private void Update()
    {
        // Actualizar UI en tiempo real
        UpdateProgressBar();
    }
    
    public void UpdateProgressBar()
    {
        if (progressBar != null && gameManager != null)
        {
            int total = gameManager.GetTotalObjects();
            int processed = gameManager.GetProcessedObjects();
            
            float progress = total > 0 ? (float)processed / total : 0;
            progressBar.value = progress;
            
            if (progressText != null)
                progressText.text = $"Objetos: {processed}/{total} ({Mathf.RoundToInt(progress * 100)}%)";
        }
    }
    
    public void UpdateEmotionalState()
    {
        if (emotionalStateSlider != null && gameManager != null)
        {
            // Calcular estado emocional basado en objetos importantes guardados vs. tirados
            int savedImportant = gameManager.GetSavedImportantObjects();
            int discardedImportant = gameManager.GetDiscardedImportantObjects();
            int total = savedImportant + discardedImportant;
        
            float emotionalValue = total > 0 ? (float)discardedImportant / total : 0.5f;
            emotionalStateSlider.value = emotionalValue;
        }
    }
    
    public void UpdateObjectCounters()
    {
        if (savedObjectsText != null)
            savedObjectsText.text = $"Guardados: {gameManager.GetSavedImportantObjects()}";
        
        if (discardedObjectsText != null)
            discardedObjectsText.text = $"Tirados: {gameManager.GetDiscardedImportantObjects()}";
    }
    
    public void ShowObjectInfo(TypeObject objectType)
    {
        if (objectInfoPanel != null && objectType != null)
        {
            objectInfoPanel.SetActive(true);
        
            if (objectNameText != null)
                objectNameText.text = objectType.name;
            
            if (objectDescriptionText != null)
                objectDescriptionText.text = objectType.GetObjectDescription();
            
            if (emotionalValueText != null)
                emotionalValueText.text = $"Valor emocional: {(objectType.isImportantObject ? "Alto" : "Bajo")}";
            
            // Animar panel
            if (uiAnimator != null)
                uiAnimator.SetTrigger("ShowObjectInfo");
        }
    }
    
    public void HideObjectInfo()
    {
        if (objectInfoPanel != null)
        {
            // Animar salida
            if (uiAnimator != null)
                uiAnimator.SetTrigger("HideObjectInfo");
            else
                objectInfoPanel.SetActive(false);
        }
    }
    
    public void UpdateActionPrompt(string promptText)
    {
        if (actionPromptText != null)
            actionPromptText.text = promptText;
    }
    
    public void ShowEndGameUI()
    {
        Debug.Log("TERMINANDO PARTIDA");
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.MinigameCompleted(6);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }
    }
}