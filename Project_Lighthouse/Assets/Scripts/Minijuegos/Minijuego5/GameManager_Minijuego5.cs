using UnityEngine;

public class GameManager_Minijuego5 : MonoBehaviour
{
    public bool[] cableConnections = new bool[6];
    public bool hasWon;

    public ParticleSystem ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasWon)
        {
            CheckVictory();
        }
    }

    private void CheckVictory()
    {
        bool b = true;
        foreach (bool cable in cableConnections)
        {
            if (!cable)
            {
                b = false;
            }
        }

        if (b)
        {
            //YOU WIN
            hasWon = true;
            ps.Play();
            FindFirstObjectByType<GameManager>().MinigameCompleted(4);
        }
    }
}
