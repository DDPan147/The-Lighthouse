using UnityEngine;

public class MinigameSwitch : MonoBehaviour
{
    public int minigameIndex;
    public bool usesStartPosition;
    public Transform startPosition;

    private Renderer rend;
    public GameObject highlight;
    private Material normalMat;
    private Material highlightMat;
    void Start()
    {
        //rend = highlight.GetComponent<Renderer>();
        //highlightMat = FindAnyObjectByType<GameManager>().highlightMat;
        //normalMat = rend.material;
    }
    public void TriggerMinigame()
    {
        FindFirstObjectByType<GameManager>().LoadMinigame(minigameIndex);
    }

    public void Awawawa(bool b)
    {
        /*if (highlight == null || rend == null) return;
        if (b)
        {
            rend.material = highlightMat;
        }
        else
        {
            rend.material = normalMat;
        }*/
        Debug.Log(b);
    }
}
