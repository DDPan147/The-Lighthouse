using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Tooltip("The amount of time the Dialogue waits between phrases")][SerializeField]
    private float waitTime = 1.0f;
    [Tooltip("The amount of frames the Dialogue waits between characters")][SerializeField]
    private int letterWaitFrames;

    private Queue<string> sentences = new Queue<string>();
    private Queue<Speaker> speakers = new Queue<Speaker>();
    private Queue<DialogueEvent> events = new Queue<DialogueEvent>();

    public enum Speaker
    {
        Abuelo,
        Luna
    }

    private Speaker lastSpeaker;
    public string[] sentencesTest = new string[] { };

    [SerializeField] private TMP_Text textDisplay;
    private AudioSource sound;

    private void Awake()
    {
        //Singleton
        /*if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);*/
        //
    }
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        sentencesTest = sentences.ToArray();
        if (Input.GetKeyDown(KeyCode.M))
        {
            CutComment();
        }
    }
    public void StartComment(DialogueComment _comment)
    {
        sentences.Clear();
        speakers.Clear();
        events.Clear();
        StopAllCoroutines();

        foreach(string sentence in _comment.sentences) 
        {
            sentences.Enqueue(sentence);
        }
        foreach (Speaker speaker in _comment.speakers)
        {
            speakers.Enqueue(speaker);
        }
        if (_comment.events.Length != 0)
        {
            foreach (DialogueEvent DialogueEvent in _comment.events)
            {
                if (DialogueEvent.eventName == "")
                {
                    DialogueEvent.active = false;
                }
                events.Enqueue(DialogueEvent);
            }
        }

        DisplayNextSentence();
    }

    void DisplayNextSentence()
    {
        if(sentences.Count == 0) 
        {
            EndComment();
            return;
        }

        string currentSentence = sentences.Dequeue();

        Speaker currentSpeaker = speakers.Dequeue();
        if(currentSpeaker != lastSpeaker)
        {
            CloseBubble(lastSpeaker);
            OpenBubble(currentSpeaker);
        }

        lastSpeaker = currentSpeaker;

        if(events.Count > 0)
        {
            DialogueEvent nEvent = events.Dequeue();

            if (nEvent.active)
            {
                if (nEvent.eventReceiver == null)
                {
                    nEvent.eventReceiver = FindFirstObjectByType<Player>();
                }
                nEvent.eventReceiver.Invoke(nEvent.eventName, nEvent.timeOffset);
            }
        }
        

        /*if (currentClip != null)
        {
            PlayVoice(currentClip);
        }*/
        //Por si se quiere que las letras salgan todas a la vez
        //textDisplay.text = currentSentence;

        // Por si se quiere que las letras salgan de una en una
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));

        //StartCoroutine(WaitForNextSentence());
    }

    /*void PlayVoice(AudioClip _clip)
    {
        sound.clip = _clip;
        sound.Play();
    }*/

    /*void StopVoice()
    {
        sound.Stop();
    }*/

    /*IEnumerator WaitForNextSentence()
    {
        do
        {
            yield return null;
        } while (sound.isPlaying);
        yield return new WaitForSeconds(waitTime);
        DisplayNextSentence();
    }*/
    
    IEnumerator TypeSentence(string _sentence)
    {
        textDisplay.text = "";
        foreach(char letter in _sentence.ToCharArray())
        {
            textDisplay.text += letter;
            for(int i = 0; i < letterWaitFrames; i++)
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(waitTime);
        DisplayNextSentence();
    }
    void EndComment()
    {
        textDisplay.text = "";
        CloseBubble(lastSpeaker);
    }

    public void CutComment()
    {
        sentences.Clear();
        events.Clear();
        StopAllCoroutines();
        textDisplay.text = "";
    }

    void OpenBubble(Speaker speaker)
    {
        //OpenBubble depending of speaker
    }
    void CloseBubble(Speaker speaker)
    {
        //CloseBubble depending of speaker
    }
}
