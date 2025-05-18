using UnityEditor;
using UnityEngine;

public class UnlockLadder : MonoBehaviour
{
    [SerializeField]private SplineSwitch[] ladders;
    public bool isOpen;

    private Renderer rend;
    public GameObject highlight;
    private Material normalMat;
    private Material highlightMat;

    private void Start()
    {
        rend = highlight.GetComponent<Renderer>();
    }
    public void UnlockTheLadder()
    {
        //Play Ladder Animation
        foreach(var ladder in ladders)
        {
            ladder.isOpen = true;
        }
    }

    public void Open()
    {
        isOpen = true;
    }
    public void SetHighlight(bool b)
    {
        if (highlight == null || rend == null) return;
        if (b)
        {
            rend.material = highlightMat;
        }
        else
        {
            rend.material = normalMat;
        }
    }
}
