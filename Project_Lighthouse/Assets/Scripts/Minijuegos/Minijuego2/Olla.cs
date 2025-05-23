using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Olla : MonoBehaviour
{
    [Header("Test Variables")]
    public bool isFilledWithFood;
    [Header("Asset Variables")]
    public GameObject foodCooked;

    [Header("Value Variables")]
    public float timeToCook;
    public int foodNeeded;
    #region Private Variables
    private int foodInPot;
    private Slider potProgress;
    [HideInInspector]public GameObject canvas;
    private VisualEffect cookingVFX;
    private Minijuego2_GameManager gm;
    [HideInInspector] public bool firstFood;
    [HideInInspector] public bool lastFood;
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
        canvas = transform.GetChild(1).gameObject;
        potProgress = canvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Slider>();
        cookingVFX = GetComponentInChildren<VisualEffect>();
    }

    private void Start()
    {
        canvas.SetActive(false);

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Comida>() != null)
        {
            Comida comida = other.gameObject.GetComponent<Comida>();
            if (comida.GetComponent<Selectable_MG2>().isGrabbed && comida.isReady)
            {
                if(comida.tipoComida == Comida.TipoComida.RestosPescado && !gm.isReciepe2)
                {
                    Debug.Log("Esta Comida No es para esto");
                    gm.ErrorComments(16,16);
                    return;
                }
                if (foodInPot == 0) firstFood = true;
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
                        canvas.SetActive(true);
                        potProgress.DOValue(1, timeToCook).OnPlay(() => cookingVFX.Play()).OnComplete(() => { isFilledWithFood = true; cookingVFX.Stop();});
                        
                    }
                    
                });
            }
            else if (!comida.isReady)
            {
                if(comida.canBePelado && !comida.isPelado && !comida.isCutted)
                {
                    Debug.Log("Falta Pelar");
                    gm.ErrorComments(4, 5);
                }
                else if(comida.canBeCutted && !comida.isCutted)
                {
                    Debug.Log("Falta Cortar");
                    gm.ErrorComments(8, 9);
                }
                else if(comida.canBeRebozado && !comida.isRebozado)
                {
                    Debug.Log("Falta Rebozar");
                    gm.ErrorComments(12, 13);
                }
                comida.transform.DOShakePosition(0.3f, 0.05f, 50, 90, false, true, ShakeRandomnessMode.Full).OnPlay(() => comida.feedbackSupervisor = false).OnComplete(() => comida.feedbackSupervisor = true);
            }
        }
    }

}
