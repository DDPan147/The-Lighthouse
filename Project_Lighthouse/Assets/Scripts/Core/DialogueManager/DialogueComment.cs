using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class Sentence
{
    [TextArea(3, 10)]
    public string sentenceText;
    public bool hasAnimation;
    public DialogueManager.Speaker speaker;
    public DialogueEvent sentenceEvent;
    public DialogueManager.Emotion emotionSound;

    public DialogueManager.AbueloFaces abueloFace;
    public DialogueManager.LunaFaces lunaFace;
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
