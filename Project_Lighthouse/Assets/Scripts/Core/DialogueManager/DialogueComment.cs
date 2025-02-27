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

    public enum DialogueTypes
    {
        Popup,
        UI
    }
    public DialogueTypes type;
}
