using UnityEngine;
using UnityEngine.UI;

public class EdgeDimEffect : MonoBehaviour
{
    [Header("Edge Dim Settings")]
    [SerializeField] private Image dimImage;
    [SerializeField] private float maxDimIntensity = 0.6f;
    [SerializeField] private float transitionSpeed = 2f;

    private float currentIntensity = 0f;
    private float targetIntensity = 0f;

    void Start()
    {
        if (dimImage == null)
            dimImage = GetComponent<Image>();

        // Configurar la imagen para que sea radial desde el centro
        dimImage.type = Image.Type.Filled;
        dimImage.fillMethod = Image.FillMethod.Radial360;
        dimImage.fillOrigin = 0;

        UpdateDimEffect();
    }

    void Update()
    {
        if (Mathf.Abs(currentIntensity - targetIntensity) > 0.01f)
        {
            currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * transitionSpeed);
            UpdateDimEffect();
        }
    }

    public void SetDimIntensity(float normalizedIntensity)
    {
        targetIntensity = Mathf.Clamp01(normalizedIntensity) * maxDimIntensity;
    }

    private void UpdateDimEffect()
    {
        if (dimImage != null)
        {
            Color color = dimImage.color;
            color.a = currentIntensity;
            dimImage.color = color;
        }
    }
}