using DG.Tweening;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

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
    private VisualEffect explodeEffect;
    private GameObject child;
    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        mainRenderer = GetComponent<Renderer>();
        mainMat = mainRenderer.material;
        rb = GetComponent<Rigidbody>();
        explodeEffect = GetComponent<VisualEffect>();
        child.SetActive(false);
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
        pointsText.DOFade(0f, 0.7f).OnComplete(() =>
        {
            
            gameObject.SetActive(false);
        });
    }

    void FadeOut()
    {
        child.SetActive(false);
        mainMat.DOFade(0f, 0.5f).OnComplete(() =>
        {
            pointsText.enabled = true;
            mainRenderer.enabled = false;
            explodeEffect.Play();
            TextFadeOut();
        });

    }

    void FadeIn()
    {
        mainMat.SetColor(Shader.PropertyToID("_BaseColor"), new Color(mainMat.GetColor("_BaseColor").r, mainMat.GetColor("_BaseColor").g, mainMat.GetColor("_BaseColor").b, 0));
        mainMat.DOFade(1, "_BaseColor" ,timeFadeIn).OnComplete(() => child.SetActive(true));
    }

    void InitialMovement()
    {
        Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-0.35f,0.35f), UnityEngine.Random.Range(-0.15f,0.15f));

        rb.linearVelocity = new Vector3(randomDirection.x, randomDirection.y, 0).normalized;
        Debug.Log(rb.linearVelocity);
    }
}
