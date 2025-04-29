using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class PruebaGlobalVolume : MonoBehaviour
{
    public VolumeProfile volumeProfile;
    private ColorAdjustments colorAdjustments;
    private int hueValue;
    void Start()
    {
        volumeProfile.TryGet<ColorAdjustments>(out colorAdjustments);
        colorAdjustments.active = true;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartDesaturacion(float time)
    {
        StopAllCoroutines();
        StartCoroutine(Desaturacion(time, hueValue));
    }

    public void StartSaturacion(float time)
    {
        StopAllCoroutines();
        StartCoroutine(Saturacion(time, hueValue));
        //colorAdjustments.saturation.value.
    }

    public void SetValue(int value)
    {
        hueValue = value;
    }

    public IEnumerator Desaturacion(float time, int valorFinal)
    {
        float iteraciones = colorAdjustments.saturation.value - valorFinal;
        if(valorFinal < 0)
        {
            while (colorAdjustments.saturation.value > valorFinal)
            {
                yield return new WaitForSecondsRealtime(time / math.abs(iteraciones));
                colorAdjustments.saturation.value--;
            }
        }
        else
        {
            Debug.LogWarning("Valor incorrecto para la desaturación");
        }
        
    }
    public IEnumerator Saturacion(float time, int valorFinal)
    {
        float iteraciones = colorAdjustments.saturation.value - valorFinal;
        if (valorFinal >= 0)
        {
            while (colorAdjustments.saturation.value < valorFinal)
            {
                yield return new WaitForSecondsRealtime(time / math.abs(iteraciones));
                colorAdjustments.saturation.value++;
                
            }
        }
        else
        {
            Debug.LogWarning("Valor incorrecto para la saturación");
        }
    }
}
