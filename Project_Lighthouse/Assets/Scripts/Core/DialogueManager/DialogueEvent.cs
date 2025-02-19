using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEvent
{
    public bool active = true;
    public string eventName;
    public MonoBehaviour eventReceiver;
    public float timeOffset;
}
