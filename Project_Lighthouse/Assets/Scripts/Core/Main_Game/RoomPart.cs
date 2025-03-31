using UnityEngine;
using DG.Tweening;

public class RoomPart : MonoBehaviour
{
    public GameObject target;
    Material mat;
    float initialAlpha;
    public float fadeTime;

    //DOTween
    Tween fadeInTween;
    Tween fadeOutTween;

    void Start()
    {
        mat = target.GetComponent<Renderer>().material;
        initialAlpha = mat.color.a;
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("B");
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
