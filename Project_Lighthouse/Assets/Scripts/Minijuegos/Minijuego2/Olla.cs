using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Olla : MonoBehaviour
{
    private GameObject gm;
    public bool isFilledWithFood;
    public GameObject foodCooked;
    private int foodInPot;
    public GameObject potProgressionBackground;
    private Image potProgress;
    public float timeToCook;
    public int foodNeeded;
    private bool _takeOffFood;
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
        gm = GameObject.Find("GameManager");
        potProgress = potProgressionBackground.transform.GetChild(0).GetComponent<Image>();
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
                gm.GetComponent<Minijuego2_GameManager>().PutFoodInPot(comida);
                gm.GetComponent<Minijuego2_GameManager>().imGrabing = false;
                PutFood.Append(other.gameObject.transform.DOMove(transform.position, 0.5f));
                PutFood.OnComplete(() =>
                {
                    foodInPot++;
                    other.gameObject.SetActive(false);
                    if (foodInPot >= foodNeeded)
                    {
                        StartCoroutine(FoodProgress());
                        
                    }
                    
                });
            }
            else
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
