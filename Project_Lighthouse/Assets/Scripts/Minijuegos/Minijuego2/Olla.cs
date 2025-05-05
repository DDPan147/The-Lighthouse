using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Olla : MonoBehaviour
{
    [Header("Test Variables")]
    public bool isFilledWithFood;
    [Header("Asset Variables")]
    public GameObject foodCooked;
    public GameObject potProgressionBackground;
    [Header("Value Variables")]
    public float timeToCook;
    public int foodNeeded;
    #region Private Variables
    private int foodInPot;
    private Image potProgress;
    private VisualEffect cookingVFX;
    private Minijuego2_GameManager gm;
    private bool _takeOffFood;
    #endregion
    [HideInInspector] public bool takeOffFood
    {
        get
        {
            return _takeOffFood;
        }
        set
        {
            _takeOffFood = value;
            if (_takeOffFood == true)
            {
                GameObject food = Instantiate(foodCooked, transform.position, Quaternion.identity);
                Sequence TakeOffFood = DOTween.Sequence();
                TakeOffFood.Append(food.transform.DOMoveY(transform.position.y + 0.5f, 0.5f));
                TakeOffFood.Append(food.transform.DOMove(transform.position + new Vector3(0, 0.2f, -0.5f), 0.5f));
            }
            
        }
    }

    private void Awake()
    {
        gm = GameObject.FindAnyObjectByType<Minijuego2_GameManager>();
        potProgress = potProgressionBackground.transform.GetChild(0).GetComponent<Image>();
        cookingVFX = GetComponentInChildren<VisualEffect>();
    }

    private void Start()
    {
        potProgress.enabled = false;
    }

    private void Update()
    {
        if (isFilledWithFood)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Comida>() != null)
        {
            Comida comida = other.gameObject.GetComponent<Comida>();
            if (comida.GetComponent<Selectable_MG2>().isGrabbed && comida.isReady)
            {
                Sequence PutFood = DOTween.Sequence();
                PutFood.Append(other.gameObject.transform.DOMove(transform.position + new Vector3(0, 0.5f, 0), 0.5f));
                gm.PutFoodInPot(comida);
                gm.imGrabing = false;
                PutFood.Append(other.gameObject.transform.DOMove(transform.position, 0.5f));
                PutFood.OnComplete(() =>
                {
                    foodInPot++;
                    other.gameObject.SetActive(false);
                    if (foodInPot >= foodNeeded)
                    {
                        potProgress.enabled = true;
                        //StartCoroutine(FoodProgress());
                        potProgress.DOFillAmount(1, timeToCook).OnPlay(() => cookingVFX.Play()).OnComplete(() => { isFilledWithFood = true; cookingVFX.Stop(); } );
                        
                    }
                    
                });
            }
            else if (!comida.isReady)
            {
                comida.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => comida.feedbackSupervisor = false).OnComplete(() => comida.feedbackSupervisor = true);
            }
        }
    }

    IEnumerator FoodProgress()
    {
        for(int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(timeToCook / 10);
            potProgress.fillAmount += 0.1f;
            if(potProgress.fillAmount >= 1)
            {
                isFilledWithFood = true;
            }
        }

    }

}
