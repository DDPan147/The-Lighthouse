using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueComment
{
    public string name;

    [TextArea(3, 10)]
    public string[] sentences;
    
    public DialogueManager.Speaker[] speakers;
    public DialogueEvent[] events;

    [Tooltip("¿Can the dialogue be reproduced infinitely or only once?")] 
    public bool isPermanent;
}
