using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueComment comment;

    public bool comentado;
    void Start()
    {

    }
    public void TriggerComment()
    {
        if (comentado == false)
        {
            //FindFirstObjectByType<DialogueManager>().StartComment(comment);
            comentado = true;
        }
    }
}
