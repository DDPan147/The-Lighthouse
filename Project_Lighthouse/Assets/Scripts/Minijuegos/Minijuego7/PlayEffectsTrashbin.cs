using UnityEngine;
using DG.Tweening; // Importar DOTween

public class PlayEffectsTrashbin : MonoBehaviour
{
    [SerializeField] private float intensidadVibracion = 0.1f;
    [SerializeField] private float durationVibration = 0.5f;
    [SerializeField] private int vibracionFuerza = 2;
    //[SerializeField] private ParticleSystem dustParticles; // Opcional para partículas

    private Vector3 originalPosition;
    private Tween vibracionTween;

    private void Start()
    {
        originalPosition = transform.position;
    }

    internal void PlayEffects()
    {
        // Detener vibración anterior si existe
        if (vibracionTween != null && vibracionTween.IsActive())
        {
            vibracionTween.Kill();
        }

        // Asegurar posición inicial correcta
        transform.position = originalPosition;

        // Crear vibración con DOTween
        vibracionTween = transform.DOShakePosition(
            durationVibration,    // Duración
            intensidadVibracion,  // Fuerza
            vibracionFuerza,      // Vibrato (número de vibraciones)
            90,                   // Aleatoriedad
            false,                // Snapping
            true                  // FadeOut
        ).OnComplete(() => {
            // Restaurar posición original al terminar
            transform.position = originalPosition;
        });

        // Reproducir partículas si existen
        //if (dustParticles != null)
        //{
        //    dustParticles.Play();
        //}
    }

    private void OnDestroy()
    {
        // Limpiar Tween al destruir el objeto
        if (vibracionTween != null && vibracionTween.IsActive())
        {
            vibracionTween.Kill();
        }
    }
}