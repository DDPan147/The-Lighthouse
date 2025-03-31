using UnityEngine;
using DG.Tweening; // Importar DOTween

public class PlayEffectsTrashbin : MonoBehaviour
{
    [SerializeField] private float intensidadVibracion = 0.1f;
    [SerializeField] private float durationVibration = 0.5f;
    [SerializeField] private int vibracionFuerza = 2;
    //[SerializeField] private ParticleSystem dustParticles; // Opcional para part�culas

    private Vector3 originalPosition;
    private Tween vibracionTween;

    private void Start()
    {
        originalPosition = transform.position;
    }

    internal void PlayEffects()
    {
        // Detener vibraci�n anterior si existe
        if (vibracionTween != null && vibracionTween.IsActive())
        {
            vibracionTween.Kill();
        }

        // Asegurar posici�n inicial correcta
        transform.position = originalPosition;

        // Crear vibraci�n con DOTween
        vibracionTween = transform.DOShakePosition(
            durationVibration,    // Duraci�n
            intensidadVibracion,  // Fuerza
            vibracionFuerza,      // Vibrato (n�mero de vibraciones)
            90,                   // Aleatoriedad
            false,                // Snapping
            true                  // FadeOut
        ).OnComplete(() => {
            // Restaurar posici�n original al terminar
            transform.position = originalPosition;
        });

        // Reproducir part�culas si existen
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