using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minijuego9_GameManager : MonoBehaviour
{
    public int totalCloudsKilled;
    public int cloudNumberToKill;
    


    void Update()
    {
        if(totalCloudsKilled >= cloudNumberToKill)
        {
            CompleteMinigame();
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            CompleteMinigame();
        }
    }

    public void CompleteMinigame()
    {
        Debug.Log("Has Ganado");
        /*Alvaro*/ //Function to complete minigame and return to lobby
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.MinigameCompleted(8);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }
    }

}
