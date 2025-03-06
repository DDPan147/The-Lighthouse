using UnityEngine;
using System.Collections.Generic;

public class MinigameSevenManager : MonoBehaviour
{
    #region Singleton
    public static MinigameSevenManager Instance { get; private set; }
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

    #region Variables
    [Header("Game State")]
    [SerializeField] private bool isGameActive = false;
    [SerializeField] private int totalObjects = 0; // Total de TODOS los objetos
    [SerializeField] private int totalImportantObjects = 0;
    [SerializeField] private int discardedImportantObjects = 0;
    [SerializeField] private int savedImportantObjects = 0;
    [SerializeField] private int processedObjects = 0; // Objetos procesados
    private bool isItemImportant = false;
    private bool isItemSaved = false;

    [Header("Lighting")]
    [SerializeField] private Light mainLight;
    [SerializeField] private float initialLightIntensity = 0.5f;
    [SerializeField] private float targetLightIntensity = 1f;
    [SerializeField] private float minLightIntensity;
    [SerializeField] private float maxLightIntensity;
    [SerializeField] private float updateLightValue;
    
    [Header("Objects Control")]
    [SerializeField] private List<GameObject> sceneObjects = new List<GameObject>();
    [SerializeField] private GameObject lastObject; // La caja que no se puede tirar
    
    [Header("UI References")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject endGameUI;

    #endregion

    #region Game Flow Methods
    private void Start()
    {
        SetupInitialState();
    }
    
    public void StartMinigame()
    {
        isGameActive = true;
        SetupInitialState();
        gameUI.SetActive(true);
        endGameUI.SetActive(false);
    }

    public void EndMinigame()
    {
        isGameActive = false;
        gameUI.SetActive(false);
        endGameUI.SetActive(true);
        CheckEndingCondition();
    }

    private void SetupInitialState()
    {
        FindAllSceneObjects();
        CountTotalImportantObjects();
        mainLight.intensity = initialLightIntensity;
        discardedImportantObjects = 0;
        savedImportantObjects = 0;
        processedObjects = 0;
        isGameActive = true;
        
        if (gameUI != null) gameUI.SetActive(true);
        if (endGameUI != null) endGameUI.SetActive(false);
    }
    private void FindAllSceneObjects()
    {
        sceneObjects.Clear();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Objeto");
        
        foreach (GameObject obj in objects)
        {
            if (obj != lastObject) // No incluimos el último objeto especial
            {
                sceneObjects.Add(obj);
            }
        }
        
        totalObjects = sceneObjects.Count;
        Debug.Log($"Total objetos encontrados: {totalObjects}");
    }
    #endregion

    #region Game Progress Tracking
    public void TrackObjectProcessed(bool isImportant, bool isSaved)
    {
        processedObjects++;
        
        if (isImportant)
        {
            isItemImportant = true;
            if (isSaved)
            {
                isItemSaved = true;
                savedImportantObjects++;
            }
            else
            {
                discardedImportantObjects++;
            }
        }
        
        UpdateGameProgress();
        Debug.Log($"Objetos procesados: {processedObjects}/{totalObjects}");
    }
    private void CountTotalImportantObjects()
    {
        TypeObject[] allObjects = FindObjectsOfType<TypeObject>();
        totalImportantObjects = 0;
        foreach (TypeObject obj in allObjects)
        {
            if (obj.isImportantObject)
            {
                totalImportantObjects++;
            }
        }
    }

    private void UpdateGameProgress()
    {
        // Actualizar iluminación basada en objetos importantes descartados
        UpdateLighting();
        
        // Verificar si todos los objetos han sido procesados
        CheckGameCompletion();
    }
    #endregion

    #region Game Completion
    private void CheckGameCompletion()
    {
        if (processedObjects >= totalObjects)
        {
            EndMinigame();
        }
    }

    private void CheckEndingCondition()
    {
        if (discardedImportantObjects > savedImportantObjects)
        {
            // Final positivo - El anciano está superando el duelo
            Debug.Log("Final positivo: El anciano está comenzando a superar la pérdida");
        }
        else
        {
            // Final negativo - El anciano sigue aferrado al pasado
            Debug.Log("Final negativo: El anciano aún no puede superar la pérdida");
        }
    }
    #endregion

    #region Lighting Control
    private void UpdateLighting()
    {
        float currentIntensity = mainLight.intensity;
        float newIntensity;

        if (isItemImportant && isItemSaved)
        {
            // Objeto importante guardado -> luz baja
            newIntensity = Mathf.Lerp(currentIntensity, minLightIntensity, updateLightValue);
            Debug.Log($"Bajando luz: {currentIntensity} -> {newIntensity}");
        }
        else if (isItemImportant && !isItemSaved)
        {
            // Objeto importante tirado -> luz sube
            newIntensity = Mathf.Lerp(currentIntensity, maxLightIntensity, updateLightValue);
            Debug.Log($"Subiendo luz: {currentIntensity} -> {newIntensity}");
        }
        else
        {
            // Objeto no importante -> mantener luz actual
            return;
        }

        mainLight.intensity = newIntensity;
    
        // Resetear las variables después de actualizar la luz
        isItemImportant = false;
        isItemSaved = false;
    }
    #endregion

    #region Public Getters
    public bool IsGameActive() => isGameActive;
    public int GetTotalObjects() => totalObjects;
    public int GetProcessedObjects() => processedObjects;
    public int GetRemainingObjects() => totalObjects - processedObjects;
    public GameObject GetLastObject() => lastObject;
    #endregion
}