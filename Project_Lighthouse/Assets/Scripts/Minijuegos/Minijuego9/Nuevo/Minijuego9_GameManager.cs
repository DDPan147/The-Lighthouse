using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minijuego9_GameManager : MonoBehaviour
{
    public int totalCloudsKilled;
    public int cloudNumberToKill;
    private GameObject jeje;
    void Start()
    {
        jeje = GameObject.Find("HoliJrjr");
    }


    void Update()
    {
        if(totalCloudsKilled >= cloudNumberToKill)
        {
            Debug.Log("Has Ganado");
            jeje.transform.DOMove(new Vector3(0, 1.6f, -9.5f), 5f);
            //
        }
    }


}
