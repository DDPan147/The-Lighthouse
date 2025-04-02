using System.Collections;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    #region Singleton
    public static LightManager Instance { get; private set; }
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
    private bool isItemImportant = false;
    private bool isItemSaved = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupLight();
    }

    private void SetupLight()
    {
        mainLight.intensity = initialLightIntensity;
    }

    #region Lighting Control
    public void UpdateLighting()
    {
        StartCoroutine(UpdateIntensityLight(increaseIntensity, maxLightIntensity, false));

        //POSIBLE LOGICA A INCORPORAR, COMPROBAR SI QUIERE QUE BAJE O SUBA DEPENDIENDO DEL SLOT EN EL QUE LO HAYA PUESTO
        //if (isItemImportant && isItemSaved)
        //{
        //    // Objeto importante guardado -> luz baja
        //    StartCoroutine(UpdateIntensityLight(decreaseIntensity, minLightIntensity, true));
        //}
        //else if (isItemImportant && !isItemSaved)
        //{
        //    // Objeto importante tirado -> luz sube
        //    StartCoroutine(UpdateIntensityLight(increaseIntensity, maxLightIntensity, false));
        //}
        //else
        //{
        //    // Objeto no importante -> mantener luz actual
        //    return;
        //}
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
        mainLight.intensity = Mathf.Clamp(mainLight.intensity, Mathf.Min(startIntensity, objectiveIntensity), Mathf.Max(startIntensity, objectiveIntensity)); ;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}
