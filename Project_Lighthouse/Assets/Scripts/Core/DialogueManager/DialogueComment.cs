using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Sentence
{
    [TextArea(3, 10)]
    public string sentenceText;
    public DialogueManager.Speaker speaker;
    public DialogueEvent sentenceEvent;
    public DialogueManager.Emotion emotionSound;
}
[System.Serializable]
public class DialogueComment
{
    public Sentence[] sentences;
    public enum DialogueTypes
    {
        Popup,
        UI
    }
    public DialogueTypes type;
}
