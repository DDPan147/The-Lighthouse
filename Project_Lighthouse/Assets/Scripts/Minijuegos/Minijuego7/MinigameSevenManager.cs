using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class MinigameSevenManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            EndMinigame();
        }
    }
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
    [SerializeField] private float actualLightIntensity;
    [SerializeField] private float initialLightIntensity = 0.5f;
    [SerializeField] private float targetLightIntensity = 1f;
    [SerializeField] private float minLightIntensity;
    [SerializeField] private float maxLightIntensity;
    [SerializeField] private float updateLightValue;
    [SerializeField] private float decreaseIntensity;
    [SerializeField] private float increaseIntensity;
    
    [Header("Objects Control")]
    [SerializeField] private List<GameObject> sceneObjects = new List<GameObject>();
    [SerializeField] private GameObject lastObject; // La caja que no se puede tirar
    [SerializeField] private MinigameComments mc;
    
    [Header("UI References")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private MinigameSevenUI _minigameSevenUIgameUI;

    [Header("Screen Dim Effect")]
    [SerializeField] private EdgeDimEffect edgeDimEffect;

    #endregion

    #region Game Flow Methods
    private void Start()
    {
        SetupInitialState();
        
        if (gameUI == null)
            _minigameSevenUIgameUI = FindObjectOfType<MinigameSevenUI>();

        mc = FindObjectOfType<MinigameComments>();
    }
    
    public void StartMinigame()
    {
        isGameActive = true;
        SetupInitialState();
        gameUI.SetActive(true);
        endGameUI.SetActive(false);
        mc.DisplayComment(0);
    }

    public void EndMinigame()
    {
        isGameActive = false;
        gameUI.SetActive(false);
        endGameUI.SetActive(true);
        if (gameUI != null) 
            gameUI.SetActive(false);
        if (endGameUI != null) 
            endGameUI.SetActive(true);
    
        bool isPositiveEnding = discardedImportantObjects > savedImportantObjects;
        if (_minigameSevenUIgameUI != null)
        {
            _minigameSevenUIgameUI.ShowEndGameUI();
        }
        else
        {
            Debug.LogWarning("minigameSevenUI no está asignado en MinigameSevenManager");
        }
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
                mc.DisplayComment(3);

                ApplyScreenDim();
            }
            else
            {
                discardedImportantObjects++;
                mc.DisplayComment(2);
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
            mc.DisplayComment(4);
            // Final positivo - El anciano está superando el duelo
            Debug.Log("Final positivo: El anciano está comenzando a superar la pérdida");
        }
        else
        {
            mc.DisplayComment(5);
            // Final negativo - El anciano sigue aferrado al pasado
            Debug.Log("Final negativo: El anciano aún no puede superar la pérdida");
        }

        // /*Alvaro*/ //Function to complete minigame and return to lobby
        // GameManager gm = FindAnyObjectByType<GameManager>();
        // if (gm != null)
        // {
        //     gm.MinigameCompleted(6);
        // }
        // else
        // {
        //     Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        // }
    }
    #endregion

    #region Lighting Control
    private void UpdateLighting()
    {
        if (isItemImportant && isItemSaved)
        {
            // Objeto importante guardado -> luz baja
            StartCoroutine(UpdateIntensityLight(decreaseIntensity, minLightIntensity, true));
        }
        else if (isItemImportant && !isItemSaved)
        {
            // Objeto importante tirado -> luz sube
            StartCoroutine(UpdateIntensityLight(increaseIntensity, maxLightIntensity, false));
        }
        else
        {
            // Objeto no importante -> mantener luz actual
            return;
        }

        // Resetear las variables después de actualizar la luz
        isItemImportant = false;
        isItemSaved = false;
    }

    private IEnumerator UpdateIntensityLight(float updateIntensity, float objectiveIntensity, bool isDecreasing)
    {
        float startIntensity = mainLight.intensity;
        float newIntensity = startIntensity + updateIntensity;

        if (isDecreasing)
            newIntensity = startIntensity - updateIntensity;

        float elapsedTime = 0f;

        while (elapsedTime < updateLightValue && Mathf.Abs(mainLight.intensity - objectiveIntensity) > 0.01f)
        {
            // Interpolación progresiva
            mainLight.intensity = Mathf.Lerp(startIntensity, newIntensity, elapsedTime / updateLightValue);
            elapsedTime += Time.deltaTime;

            // Esperar al siguiente frame
            yield return null;
        }

        // Asegurarse de que la intensidad final sea exactamente la deseada
        mainLight.intensity = Mathf.Clamp(mainLight.intensity, Mathf.Min(startIntensity, objectiveIntensity), Mathf.Max(startIntensity, objectiveIntensity));;
    }

    private void ApplyScreenDim()
    {
        if (edgeDimEffect != null)
        {
            // Calcular intensidad basada en objetos importantes guardados
            float dimProgress = (float)savedImportantObjects / totalImportantObjects;
            edgeDimEffect.SetDimIntensity(dimProgress);
        }
    }
    #endregion

    #region Public Getters
    public bool IsGameActive() => isGameActive;
    public int GetTotalObjects() => totalObjects;
    public int GetProcessedObjects() => processedObjects;
    public int GetRemainingObjects() => totalObjects - processedObjects;
    public GameObject GetLastObject() => lastObject;
    
    public int GetSavedImportantObjects() => savedImportantObjects;
    public int GetDiscardedImportantObjects() => discardedImportantObjects;
    #endregion
}