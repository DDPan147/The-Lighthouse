using DG.Tweening;
using TMPro;
using UnityEngine;

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
    private void Awake()
    {
        mainRenderer = GetComponent<Renderer>();
        mainMat = mainRenderer.material;
    }

    void Start()
    {
        pointsText.enabled = false;
        initialScale = transform.localScale;
        FadeIn();
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
        // Movimiento Inicial
    }
}
