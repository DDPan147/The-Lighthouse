using UnityEngine;
using DG.Tweening; // Importar DOTween

public class PlayEffectsTrashbin : MonoBehaviour
{
    [SerializeField] private float intensidadVibracion = 0.1f;
    [SerializeField] private float durationVibration = 0.5f;
    [SerializeField] private int vibracionFuerza = 2;
    [SerializeField] private int vibraciones = 10;
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

        // Establecemos uan direccion de vibracion para poder realizar la animacion
        Vector3 direccionVibracion = new Vector3(intensidadVibracion, 0, 0);

        // DOPunchPosition para crear un efecto de "vibracion"
        vibracionTween = transform.DOPunchPosition(
            direccionVibracion,   // Direccion de la vibracion (izquierda a derecha)
            durationVibration,    // Duracion de la animacon
            vibraciones,          // Numero de vibraciones
            0.5f                  // Elasticidad (0-1)
        ).OnComplete(() => {
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