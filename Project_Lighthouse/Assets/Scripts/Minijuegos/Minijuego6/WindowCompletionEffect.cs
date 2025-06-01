using UnityEngine;
using DG.Tweening;

public class WindowCompletionEffect : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Renderer windowRenderer;
    [SerializeField] private Material windowMaterial;
    
    [Header("Configuración del Destello")]
    [SerializeField] private Color highlightColor = Color.white;
    [SerializeField] private float highlightIntensity = 2f;
    public float sweepDuration = 1.5f;
    [SerializeField] private float sweepWidth = 0.2f;
    
    private static readonly int SweepPositionID = Shader.PropertyToID("_SweepPosition");
    private static readonly int HighlightColorID = Shader.PropertyToID("_HighlightColor");
    private static readonly int HighlightIntensityID = Shader.PropertyToID("_HighlightIntensity");
    private static readonly int SweepWidthID = Shader.PropertyToID("_SweepWidth");
    
    private void Awake()
    {
        if (windowRenderer == null)
            windowRenderer = GetComponent<Renderer>();
            
        if (windowMaterial == null && windowRenderer != null)
            windowMaterial = windowRenderer.material;
    }
    
    public void PlayCompletionEffect()
    {
        if (windowMaterial == null)
            return;
        // Asegurarse de que el material tiene las propiedades necesarias
        windowMaterial.SetColor(HighlightColorID, highlightColor);
        windowMaterial.SetFloat(HighlightIntensityID, highlightIntensity);
        windowMaterial.SetFloat(SweepWidthID, sweepWidth);
        
        // Iniciar el destello desde fuera del cuadro (izquierda)
        windowMaterial.SetFloat(SweepPositionID, -1f);
        
        // Animar la posición del destello de izquierda a derecha
        DOTween.To(() => -1f, x => windowMaterial.SetFloat(SweepPositionID, x), 2f, sweepDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => {
                // Restablecer la posición del destello fuera de la vista
                windowMaterial.SetFloat(SweepPositionID, -1f);
                windowMaterial.SetFloat(HighlightIntensityID, 0f);
            });
    }
}