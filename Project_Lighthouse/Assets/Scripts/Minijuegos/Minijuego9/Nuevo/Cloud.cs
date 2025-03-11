using DG.Tweening;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour
{
    [HideInInspector] public bool isDying
    {
        get
        {
            return isDying;
        }
        set
        {
            if(value == true)
            {
                FadeOut();
            }
        }
    }
    [HideInInspector] public bool isInvincible = false;
    [HideInInspector] public float timeLighting;
    [HideInInspector] public Vector3 initialScale;
    private Renderer mainRenderer;
    private Material mainMat;
    public int points;
    public TMP_Text pointsText;
    public float timeFadeIn;
    private Rigidbody rb;
    private void Awake()
    {
        mainRenderer = GetComponent<Renderer>();
        mainMat = mainRenderer.material;
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        pointsText.enabled = false;
        initialScale = transform.localScale;
        FadeIn();
        InitialMovement();
    }

    void TextFadeOut()
    {
        pointsText.DOFade(0f, 0.4f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    void FadeOut()
    {
        mainMat.DOFade(0f, 0.5f).OnComplete(() =>
        {
            pointsText.enabled = true;
            mainRenderer.enabled = false;
            TextFadeOut();
        });

    }

    void FadeIn()
    {
        mainMat.color = new Color(mainMat.color.r, mainMat.color.g, mainMat.color.b, 0);
        mainMat.DOFade(1, timeFadeIn);
    }

    void InitialMovement()
    {
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1.0f,1.0f), UnityEngine.Random.Range(-1.0f,1.0f));
        //Linear velocity
        rb.linearVelocity = new Vector3(randomDirection.x, randomDirection.y, 0);
        //Add Force

    }
}
