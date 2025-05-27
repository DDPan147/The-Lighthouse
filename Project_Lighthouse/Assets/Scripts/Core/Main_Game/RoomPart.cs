using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;
using System;
using Unity.VisualScripting;
using System.Collections;

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

    private Player player;
    //DOTween
    public int numberOfTweens;
    public List<Tween> fadeInTweens = new List<Tween>();
    public List<Tween> fadeOutTweens = new List<Tween>();

    void Start()
    {
        for(int i = 0; i < targets.Length; i++)
        {
            targets[i].mat = targets[i].target.GetComponent<Renderer>().material;
            //targets[i].initialAlpha = targets[i].mat.color.a;
            targets[i].initialAlpha = targets[i].mat.GetFloat("_Alpha");
        }
        player = FindAnyObjectByType<Player>();
    }

    void Update()
    {
        numberOfTweens = fadeInTweens.Count;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " has detected: " + other.gameObject.name + " with tag " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Player"))
        {
            fadeInTweens.Clear();

            player.currentRoom = this;

            if(fadeOutTweens.Count > 0)
            {
                foreach (Tween fadeOutTween in fadeOutTweens)
                {
                    fadeOutTween.Pause();
                }
            }

            foreach (FadeTarget target in targets)
            {
                //Tween fadeInTween = DOTween.ToAlpha(() => target.mat.color, x => target.mat.color = x, target.amount, fadeTime).SetEase(Ease.OutCubic);
                Tween fadeInTween = target.mat.DOFloat(target.amount, "_Alpha", fadeTime);
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

        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(TriggerExitDelay());
        }
    }

    IEnumerator TriggerExitDelay()
    {
        yield return null;
        fadeOutTweens.Clear();

        if (fadeInTweens.Count > 0)
        {
            foreach (Tween fadeInTween in fadeInTweens)
            {
                fadeInTween.Pause();
            }
        }

        foreach (FadeTarget target in targets)
        {
            if(player.currentRoom == this)
            {
                player.currentRoom = null;
            }
            if (player.currentRoom != null)
            {
                if (Array.Exists(player.currentRoom.targets, newTarget => newTarget.target == target.target))
                {
                    continue;
                }
            }


            //Tween fadeOutTween = DOTween.ToAlpha(() => target.mat.color, x => target.mat.color = x, target.initialAlpha, fadeTime).SetEase(Ease.OutCubic);
            Tween fadeOutTween = target.mat.DOFloat(target.initialAlpha, "_Alpha", fadeTime);
            fadeOutTweens.Add(fadeOutTween);
        }
    }
}
