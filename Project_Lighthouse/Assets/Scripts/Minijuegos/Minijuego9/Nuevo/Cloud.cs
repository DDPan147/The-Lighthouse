using DG.Tweening;
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
               gameObject.SetActive(false);
            }
        }
    }
    [HideInInspector] public bool isInvincible = false;
    [HideInInspector] public float timeLighting;
    [HideInInspector] public Vector3 initialScale;
    public int points;

    void Start()
    {
        initialScale = transform.localScale;
    }


    void Update()
    {

    }
}
