using UnityEngine;

public class Cuchara : MonoBehaviour
{
    [HideInInspector] public bool hasFood;
    private bool thereIsDish;
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
        if (other.gameObject.CompareTag("Plato") && hasFood)
        {
            other.transform.GetChild(1).gameObject.SetActive(true);
            gm.finishRecipe2 = true;
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Plato"))
        {
            thereIsDish = false;

        }
    }
}
