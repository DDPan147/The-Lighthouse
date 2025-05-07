using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class RoomPart : MonoBehaviour
{
    /*Fade Rooms*/
    [System.Serializable]
    public struct FadeTarget
    {
        public FadeTarget(GameObject _target, float _amount)
        {
            target = _target;
            amount = _amount;
            mat = null;
            initialAlpha = 1f;
        }
        public GameObject target;
        [HideInInspector]
        public Material mat;
        [Range(0,1)]
        public float amount;
        [HideInInspector]
        public float initialAlpha;
    }

    public FadeTarget[] targets;
    public float fadeTime;

    //DOTween
    List<Tween> fadeInTweens = new List<Tween>();
    List<Tween> fadeOutTweens = new List<Tween>();

    void Start()
    {
        for(int i = 0; i < targets.Length; i++)
        {
            targets[i].mat = targets[i].target.GetComponent<Renderer>().material;
            targets[i].initialAlpha = targets[i].mat.color.a;
        }
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
            if(fadeOutTweens.Count > 0)
            {
                foreach (Tween fadeOutTween in fadeOutTweens)
                {
                    fadeOutTween.Pause();
                }
            }

            foreach(FadeTarget target in targets)
            {
                Tween fadeInTween = DOTween.ToAlpha(() => target.mat.color, x => target.mat.color = x, target.amount, fadeTime).SetEase(Ease.OutCubic);
                fadeInTweens.Add(fadeInTween);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        /*if (other.gameObject.CompareTag("Player"))
        {
            fadeInTween.Pause();
            fadeOutTween = DOTween.ToAlpha(() => mat.color, x => mat.color = x, initialAlpha, fadeTime).SetEase(Ease.InCubic);
        }*/

        if (fadeInTweens.Count > 0)
        {
            foreach (Tween fadeInTween in fadeInTweens)
            {
                fadeInTween.Pause();
            }
        }

        foreach (FadeTarget target in targets)
        {
            Tween fadeOutTween = DOTween.ToAlpha(() => target.mat.color, x => target.mat.color = x, target.initialAlpha, fadeTime).SetEase(Ease.OutCubic);
            fadeOutTweens.Add(fadeOutTween);
        }
    }
}
