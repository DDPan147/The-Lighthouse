using TMPro;
using UnityEngine;

public class MinigameComments : MonoBehaviour
{
    private DialogueManager dm;
    [SerializeField] private MinigameComment[] comments;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dm = FindAnyObjectByType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayComment(int index)
    {
        if (dm != null && !comments[index].commented)
        {
            dm.DisplayGUIComment(comments[index]);
            comments[index].commented = true;
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Dialogue Manager de la escena principal.");
        }
    }
}
