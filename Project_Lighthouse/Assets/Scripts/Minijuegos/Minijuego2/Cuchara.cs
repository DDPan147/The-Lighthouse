using UnityEngine;

public class Cuchara : MonoBehaviour
{
    [HideInInspector] public bool hasFood;
    [HideInInspector]public bool thereIsPot;
    [HideInInspector]public bool thereIsPlato;
    [HideInInspector]public Olla olla;
    //[HideInInspector]public GameObject plato;
    private Minijuego2_GameManager gm;
    private void Awake()
    {
        gm = GameObject.FindAnyObjectByType<Minijuego2_GameManager>();
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Olla"))
        {
            olla = other.GetComponent<Olla>();
            if(olla.isFilledWithFood)
            {
                thereIsPot = true;
            }
            
        }
        if (other.gameObject.CompareTag("Plato"))
        {
            thereIsPlato = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Olla"))
        {
            thereIsPot = false;
        }
        if (other.gameObject.CompareTag("Plato"))
        {
            thereIsPlato = false;
        }
    }

    public void PutSoup(GameObject plato)
    {
        plato.transform.GetChild(1).gameObject.SetActive(true);
        gm.finishRecipe2 = true;
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void GetSoup()
    {
        Debug.Log("Sopita");
        hasFood = true;
        transform.GetChild(1).gameObject.SetActive(true);
        olla.GetComponent<Olla>().canvas.SetActive(false);
        olla.GetComponent<Olla>().isFilledWithFood = false;
        olla.GetComponent<Olla>().lastFood = true;
    }

}
