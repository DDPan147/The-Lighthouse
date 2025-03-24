using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Olla : MonoBehaviour
{
    private GameObject gm;
    public bool isFilledWithFood;
    private int foodInPot;
    public GameObject potProgressionBackground;
    private Image potProgress;
    public float timeToCook;

    private void Awake()
    {
        gm = GameObject.Find("GameManager");
        potProgress = potProgressionBackground.transform.GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if (isFilledWithFood)
        {
            Debug.Log("La comida esta lista");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Comida_Cortada>() != null)
        {
            Comida_Cortada comida = other.gameObject.GetComponent<Comida_Cortada>();
            if (comida.isGrabbed && comida.isCutted && comida.isRebozado)
            {
                Sequence PutFood = DOTween.Sequence();
                PutFood.Append(other.gameObject.transform.DOMove(transform.position + new Vector3(0, 0.5f, 0), 0.5f));
                gm.GetComponent<Minijuego2_GameManager>().PutFoodInPot(comida);
                PutFood.Append(other.gameObject.transform.DOMove(transform.position, 0.5f));
                PutFood.OnComplete(() =>
                {
                    foodInPot++;
                    gm.GetComponent<Minijuego2_GameManager>().imGrabing = false;
                    if (foodInPot >= 2)
                    {
                        StartCoroutine(FoodProgress());
                        isFilledWithFood = true;
                    }
                    
                });
            }
        }
    }

    IEnumerator FoodProgress()
    {
        for(int i = 0; i < 10; i++)
        {
            Debug.Log("Hola");
            yield return new WaitForSeconds(timeToCook / 10);
            Debug.Log("caracola");
            potProgress.fillAmount += 1 / 10;
        }
    }

}
