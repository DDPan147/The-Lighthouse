using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueComment comment;

    public bool comentado;
    private Player player;
    void Start()
    {
        player = FindAnyObjectByType<Player>();
    }
    public void TriggerComment()
    {
        if (comentado == false)
        {
            if (player.moveState != Player.MoveStates.Control) return;

            FindAnyObjectByType<DialogueManager>().StartComment(comment);
            comentado = true;
        }
    }
}
