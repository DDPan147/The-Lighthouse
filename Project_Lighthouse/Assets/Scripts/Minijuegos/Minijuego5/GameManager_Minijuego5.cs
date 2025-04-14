using UnityEngine;

public class GameManager_Minijuego5 : MonoBehaviour
{
    public bool[] cableConnections = new bool[6];
    public bool[] cableCorrectConnections = new bool[6];
    public bool hasWon;

    public DragAndDrop_Cable[] cables;
    public ParticleSystem ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cables = FindObjectsByType<DragAndDrop_Cable>(FindObjectsSortMode.InstanceID);
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
            bool a = true;
            foreach (bool cable in cableCorrectConnections)
            {
                if (!cable)
                {
                    a = false;
                }
            }
            if (a)
            {
                //YOU WIN
                hasWon = true;
                ps.Play();
                GameManager gm = FindAnyObjectByType<GameManager>();
                if (gm != null)
                {
                    gm.MinigameCompleted(4);
                }
                else
                {
                    Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
                }
            }
            else
            {
                foreach(DragAndDrop_Cable cable in cables)
                {
                    cable.ResetPosition();
                }
                cableConnections = new bool[6];
                cableCorrectConnections = new bool[6];
            }
        }
    }

    public int CurrentCableConnections()
    {
        int output = 0;
        foreach (bool cable in cableConnections)
        {
            if (cable)
            {
                output++;
            }
        }
        Debug.Log(output);
        return output;
    }
}
