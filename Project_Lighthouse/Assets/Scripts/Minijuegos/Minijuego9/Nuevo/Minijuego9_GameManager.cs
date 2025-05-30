using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minijuego9_GameManager : MonoBehaviour
{
    public int totalCloudsKilled;
    public int cloudNumberToKill;
    [HideInInspector] public MinigameComments mc;

    private void Awake()
    {
        mc = GetComponent<MinigameComments>();
    }

    void Update()
    {
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

    public IEnumerator FinishMinigame9Sequence()
    {
        mc.DisplayComment(7);
        yield return new WaitForSecondsRealtime(2f);
        mc.DisplayComment(8);
        yield return new WaitForSecondsRealtime(2f);
        mc.DisplayComment(9);
        yield return new WaitForSecondsRealtime(2f);
        mc.DisplayComment(10);
        yield return new WaitForSecondsRealtime(2f);
        mc.DisplayComment(11);
        yield return new WaitForSecondsRealtime(2f);
        mc.DisplayComment(12);
        yield return new WaitForSecondsRealtime(2f);
        mc.DisplayComment(13);
        yield return new WaitForSecondsRealtime(3f);
        mc.DisplayComment(14);
        yield return new WaitForSecondsRealtime(3f);
        mc.DisplayComment(15);
        yield return new WaitForSecondsRealtime(2.5f);
        CompleteMinigame();
    }


}
