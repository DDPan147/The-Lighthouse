using UnityEngine;
using DG.Tweening;

public class RoomPart : MonoBehaviour
{
    Material mat;
    float initialAlpha;
    public float fadeTime;

    //DOTween
    Tween fadeInTween;
    Tween fadeOutTween;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        initialAlpha = mat.color.a;
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.CompareTag("Player"))
        {
            fadeOutTween.Pause();
            fadeInTween = DOTween.ToAlpha(() => mat.color, x => mat.color = x, 0, fadeTime).SetEase(Ease.OutCubic);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fadeInTween.Pause();
            fadeOutTween = DOTween.ToAlpha(() => mat.color, x => mat.color = x, initialAlpha, fadeTime).SetEase(Ease.InCubic);
        }
    }
}
